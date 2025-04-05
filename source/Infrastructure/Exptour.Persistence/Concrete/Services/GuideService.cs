using AutoMapper;
using Exptour.Application.Abstract.Repositories.Guides;
using Exptour.Application.Abstract.Repositories.GuideSchedules;
using Exptour.Application.Abstract.Repositories.Languages;
using Exptour.Application.Abstract.Services;
using Exptour.Application.BackgroundJobs.Guide;
using Exptour.Application.DTOs.Guide;
using Exptour.Application.DTOs.User;
using Exptour.Application.Validators.Guide;
using Exptour.Common.Helpers;
using Exptour.Common.Infrastructure.Services;
using Exptour.Common.Shared;
using Exptour.Domain;
using Exptour.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Quartz;
using System.Net;
using System.Text;
using static Exptour.Application.Constants.ExceptionMessages;
using static Exptour.Application.Constants.ServiceSettingsKeys;

namespace Exptour.Persistence.Concrete.Services;

public class GuideService : BaseService, IGuideService
{
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly IGuideReadRepository _guideReadRepository;
    private readonly IGuideWriteRepository _guideWriteRepository;
    private readonly IGuideScheduleWriteRepository _guideScheduleWriteRepository;
    private readonly IGuideScheduleReadRepository _guideScheduleReadRepository;
    private readonly ILanguageReadRepository _languageReadRepository;
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IMailService _mailService;
    private readonly IUserService _userService;

    public GuideService(IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration,
        IMapper mapper,
        IGuideReadRepository guideReadRepository,
        IGuideWriteRepository guideWriteRepository,
        ILanguageReadRepository languageReadRepository,
        ISchedulerFactory schedulerFactory,
        IMailService mailService,
        IUserService userService,
        IGuideScheduleWriteRepository guideScheduleWriteRepository,
        IGuideScheduleReadRepository guideScheduleReadRepository) : base(httpContextAccessor, configuration)
    {
        _configuration = configuration;
        _mapper = mapper;
        _guideReadRepository = guideReadRepository;
        _guideWriteRepository = guideWriteRepository;
        _languageReadRepository = languageReadRepository;
        _schedulerFactory = schedulerFactory;
        _mailService = mailService;
        _userService = userService;
        _guideScheduleWriteRepository = guideScheduleWriteRepository;
        _guideScheduleReadRepository = guideScheduleReadRepository;
    }

    public async Task<APIResponse<SetScheduleResponse>> AssignScheduleAsync(string id, DateTime startDate, DateTime endDate)
    {
        var response = new APIResponse<SetScheduleResponse>();

        var currentLanguage = GetCurrentLanguage();

        if (!id.IsValidGuid())
        {
            var msgInvalidRequest = GetMessageByLocalization(InvalidRequest);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var guide = await _guideReadRepository.FirstOrDefaultAsync(g => g.Id == Guid.Parse(id) && !g.IsDeleted
            && g.Status == GuideStatus.Active,
            include: g => g.Include(g => g.GuideSchedules));
        if (guide is null)
        {
            var msgNotFound = GetMessageByLocalization(NotFound);
            response.Message = msgNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgNotFound.state;
            return response;
        }

        startDate = startDate.ToUTC();
        endDate = endDate.ToUTC();
        var today = DateTime.UtcNow.AddDays(-1);
        if (startDate.Date < today.Date || endDate.Date < today.Date || startDate >= endDate)
        {
            var msgInvalidRequest = GetMessageByLocalization(InvalidDate);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        bool isOverlapping = guide.GuideSchedules.Any(s => (s.StartDate <= endDate && s.EndDate >= startDate) && !s.IsDeleted);
        if (isOverlapping)
        {
            var msgInvalidRequest = GetMessageByLocalization(AlreadyAssignedInThisPeriod);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        await _guideScheduleWriteRepository.AddAsync(new Domain.Entities.GuideSchedule()
        {
            Id = Guid.NewGuid(),
            GuideId = guide.Id,
            StartDate = startDate,
            EndDate = endDate
        });

        var hasActiveSchedule = guide.GuideSchedules
            .Any(s => s.StartDate <= today && s.EndDate >= today && !s.IsDeleted);

        var hasUpcomingSchedule = guide.GuideSchedules
            .Any(s => s.StartDate.Date <= today.AddDays(int.Parse(_configuration[MaxDaysBetweenSchedules])).Date
                && s.StartDate.Date >= today.Date && !s.IsDeleted);

        guide.IsAvailable = !hasActiveSchedule && !hasUpcomingSchedule;

        _guideWriteRepository.Update(guide);
        await _guideScheduleWriteRepository.SaveChangesAsync();

        await _mailService.SendMailAsync(new string[] { guide.Email }, "New Tour Schedule Assigned",
            GetScheduleEmailBody(guide.FullName, startDate.ToUAE(), endDate.ToUAE()));

        var scheduler = await _schedulerFactory.GetScheduler();

        var job = JobBuilder.Create<GuideAvailabilityJob>()
            .WithIdentity($"GuideAvailabilityJob-{guide.Id.ToString()}")
            .UsingJobData("GuideId", guide.Id.ToString())
            .Build();

        var nearingScheduleDay = (startDate - today).Days;
        var trigger = TriggerBuilder.Create()
            .WithIdentity($"GuideAvailabilityTrigger-{guide.Id.ToString()}")
            .StartAt(DateTimeOffset.UtcNow.AddDays(nearingScheduleDay - 1))
            .Build();

        response.Payload = new SetScheduleResponse(guide.Id,
            guide.IsAvailable,
            startDate.ToUAE().ToString("yyyy-MM-dd"),
            endDate.ToUAE().ToString("yyyy-MM-dd"));
        return response;
    }

    public async Task<APIResponse<AssignLanguageResponse>> AssignLanguageAsync(List<string> languageIds)
    {
        var response = new APIResponse<AssignLanguageResponse>();

        var curentLanguage = GetCurrentLanguage();

        var authData = GetAuthData();

        if (string.IsNullOrEmpty(authData.authId) || !authData.authId.IsValidGuid())
        {
            var msgUnauthorized = GetMessageByLocalization(UnauthorizedUser);
            response.ResponseCode = HttpStatusCode.Unauthorized;
            response.Message = msgUnauthorized.message;
            response.State = msgUnauthorized.state;
            return response;
        }

        authData.authId = await _userService.GetGuideIdByUserIdAsync(authData.authId);
        if (string.IsNullOrEmpty(authData.authId) || !authData.authId.IsValidGuid())
        {
            var msgUnauthorized = GetMessageByLocalization(UnauthorizedUser);
            response.ResponseCode = HttpStatusCode.Unauthorized;
            response.Message = msgUnauthorized.message;
            response.State = msgUnauthorized.state;
            return response;
        }

        var guide = await _guideReadRepository.FirstOrDefaultAsync(g => g.Id == Guid.Parse(authData.authId) && !g.IsDeleted
            && g.Status == GuideStatus.Active,
            include: g => g.Include(g => g.Languages));
        if (guide is null)
        {
            var msgNotFound = GetMessageByLocalization(NotFound);
            response.Message = msgNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgNotFound.state;
            return response;
        }

        if (languageIds.Any(l => !l.IsValidGuid()))
        {
            var msgInvalidRequest = GetMessageByLocalization(InvalidRequest);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var languages = await _languageReadRepository.Table.Where(l => languageIds.Contains(l.Id.ToString()) && !l.IsDeleted).ToListAsync();
        if (languages.Count != languageIds.Count)
        {
            var msgNotFound = GetMessageByLocalization(OneOrMoreLanguagesCanNotBeFound);
            response.Message = msgNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgNotFound.state;
            return response;
        }

        var guideExistedLanguages = guide.Languages.Select(l => l.Id).ToList();
        var newLanguages = languages.Where(l => !guideExistedLanguages.Contains(l.Id)).ToList();
        if (newLanguages.Count == 0 || newLanguages is null)
        {
            var msgNotFound = GetMessageByLocalization(ThereAreNoNewLanguages);
            response.Message = msgNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgNotFound.state;
            return response;
        }

        foreach (var language in newLanguages)
            guide.Languages.Add(language);
        _guideWriteRepository.Update(guide);
        await _guideWriteRepository.SaveChangesAsync();

        response.Payload = new AssignLanguageResponse(guide.Id,
            guide.FullName,
            guide.Email,
            guide.Languages.Select(l => Helper.GetByLanguage(l.NameEN, l.NameAR, curentLanguage)).ToList());
        return response;
    }

    public async Task<APIResponse<CreateGuideResponse>> CreateAsync(CreateGuideDTO createGuideDTO)
    {
        var response = new APIResponse<CreateGuideResponse>();

        CreateGuideDTOValidator validations = new();

        var result = await validations.ValidateAsync(createGuideDTO);
        if (!result.IsValid)
        {
            StringBuilder stringBuilder = new();
            foreach (var error in result.Errors)
                stringBuilder.AppendLine(error.ErrorMessage);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = stringBuilder.ToString();
            response.State = GetMessageByLocalization(InvalidRequest).state;
            return response;
        }

        if (createGuideDTO.LanguageIds.Count < int.Parse(_configuration[BasicViolation]))
        {
            var msgInvalidRequest = GetMessageByLocalization(MustHaveAtLeastTwoLanguages);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var id = await _userService.GetUserIdByEmailAsync(createGuideDTO.Email);
        if (id is not null)
        {
            var userRoles = await _userService.GetRolesAccordingToUserAsync(id);
            if (userRoles.Payload is not null && userRoles.Payload.Contains("Guide"))
            {
                var msgGuideExists = GetMessageByLocalization(AlreadyExists);
                response.Message = msgGuideExists.message;
                response.ResponseCode = HttpStatusCode.BadRequest;
                response.State = msgGuideExists.state;
                return response;
            }
        }

        if (await _guideReadRepository.Table.AsNoTracking()
            .AnyAsync(g => (g.FullName.ToLower() == createGuideDTO.FullName.ToLower() || g.Email.ToLower() == createGuideDTO.Email.ToLower())
            && !g.IsDeleted
            && g.Status == GuideStatus.Active))
        {
            var msgGuideExists = GetMessageByLocalization(AlreadyExists);
            response.Message = msgGuideExists.message;
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.State = msgGuideExists.state;
            return response;
        }

        await using var transaction = await _guideWriteRepository.BeginTransactionAsync();
        try
        {

            var guide = new Guide()
            {
                FullName = createGuideDTO.FullName.ToProperFullName(),
                Email = createGuideDTO.Email,
                Gender = createGuideDTO.Gender,
            };

            await _guideWriteRepository.AddAsync(guide);
            await _guideWriteRepository.SaveChangesAsync();

            var guideUser = await _userService.RegisterAsync(new RegisterDTO(guide.FullName.ToProperLastName(),
                guide.FullName.ToProperFirstName(),
                guide.Email,
                createGuideDTO.Password,
                guide.Gender));

            if (guideUser.Payload is null || string.IsNullOrEmpty(guideUser.Payload.UserId.ToString()))
            {
                try
                {
                    var userId = await _userService.GetUserIdByEmailAsync(guide.Email);
                    if (userId is null)
                    {
                        var resultRegister = await _userService.RegisterAsync(new RegisterDTO(guide.FullName.ToProperLastName(), guide.FullName.ToProperFirstName(), guide.Email, createGuideDTO.Password, guide.Gender));
                        if (resultRegister.Payload is null || string.IsNullOrEmpty(resultRegister.Payload.UserId.ToString()))
                        {
                            await transaction.RollbackAsync();
                            response.ResponseCode = HttpStatusCode.BadRequest;
                            response.Message = resultRegister.Message;
                            response.State = resultRegister.State;
                            return response;
                        }

                        userId = resultRegister.Payload.UserId.ToString();
                    }
                    await _userService.AssignRoleToUserAsnyc(new AssignRoleDTO(userId, new List<string> { "Guide" }));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    response.ResponseCode = HttpStatusCode.BadRequest;
                    response.Message = ex.Message;
                    response.State = GetMessageByLocalization(SomethingWentWrong).state;
                    return response;
                }
            }
            else
                await _userService.AssignRoleToUserAsnyc(new AssignRoleDTO(guideUser.Payload.UserId.ToString(), new List<string> { "Guide" }));

            var resultAssignLanguage = await AssignLanguageAsync(guide.Id.ToString(), createGuideDTO.LanguageIds);
            if (resultAssignLanguage.ResponseCode is not HttpStatusCode.OK)
            {
                await transaction.RollbackAsync();
                response.ResponseCode = resultAssignLanguage.ResponseCode;
                response.Message = resultAssignLanguage.Message;
                response.State = resultAssignLanguage.State;
                return response;
            }

            await transaction.CommitAsync();

            response.Payload = new CreateGuideResponse(guide.Id, guide.FullName, guide.Email);
            return response;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            response.ResponseCode = HttpStatusCode.InternalServerError;
            response.Message = ex.Message;
            response.State = GetMessageByLocalization(SomethingWentWrong).state;
            return response;
        }
    }

    public async Task<APIResponse<GuideResponse>> GetByIdAsync(string id)
    {
        var response = new APIResponse<GuideResponse>();

        if (!id.IsValidGuid())
        {
            var msgInvalidRequest = GetMessageByLocalization(InvalidRequest);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var guide = await _guideReadRepository.FirstOrDefaultAsync(g => g.Id == Guid.Parse(id) && !g.IsDeleted
            && g.Status == GuideStatus.Active,
            include: g => g.Include(g => g.Languages)
                           .Include(g => g.GuideSchedules)
                           .AsSplitQuery());
        if (guide is null)
        {
            var msgNotFound = GetMessageByLocalization(NotFound);
            response.Message = msgNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgNotFound.state;
            return response;
        }

        var guideLanguages = guide.Languages
            .Select(l => Helper.GetByLanguage(l.NameEN, l.NameAR, GetCurrentLanguage())).ToList();

        var guideLastSchedule = guide.GuideSchedules
            .OrderByDescending(s => s.EndDate)
            .FirstOrDefault();

        var today = DateTime.UtcNow.AddDays(-1);
        response.Payload = new GuideResponse(guide.Id,
            guide.FullName,
            guide.Email,
            guide.Gender == Gender.Male ? "Male" : (guide.Gender == Gender.Female ? "Female" : "Other"),
            guideLanguages,
            guide.IsAvailable,
            guide.Status == Domain.GuideStatus.Active ? true : false,
            guideLastSchedule is not null ? $"{guideLastSchedule.StartDate.Date.ToUAE().AddDays(1).ToString("yyyy-MM-dd")} - {guideLastSchedule.EndDate.Date.ToUAE().AddDays(1).ToString("yyyy-MM-dd")}" : null,
            guide.GuideSchedules.Where(s => s.StartDate < today && !s.IsDeleted).Count(),
            guide.GuideSchedules.Where(s => s.StartDate >= today && !s.IsDeleted).Count());
        return response;
    }

    public async Task<APIResponse<GuideStatisticsResponse>> GetGuidesStatisticsAsync()
    {
        var response = new APIResponse<GuideStatisticsResponse>();

        var totalGuides = await _guideReadRepository.GetAll(g => !g.IsDeleted && g.Status == Domain.GuideStatus.Active,
                include: g => g.Include(g => g.Languages)
                               .Include(g => g.GuideSchedules)
                               .AsSplitQuery())
            .ToListAsync();

        var totalGuidesCount = totalGuides.Count;
        if (totalGuidesCount == 0 || totalGuides is null)
        {
            var msgNotFound = GetMessageByLocalization(NotFound);
            response.Message = msgNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgNotFound.state;
            return response;
        }

        var totalSchedulesCount = await _guideScheduleReadRepository.Table
            .Where(s => !s.IsDeleted)
            .AsNoTracking()
            .CountAsync();

        var maleAssignmentCount = totalGuides.Where(g => g.Gender == Gender.Male)
            .Sum(g => g.GuideSchedules.Count());
        var femaleAssignmentCount = totalGuides.Where(g => g.Gender == Gender.Female)
            .Sum(g => g.GuideSchedules.Count());
        var maleCount = totalGuides.Where(g => g.Gender == Gender.Male).Count();
        var femaleCount = totalGuides.Where(g => g.Gender == Gender.Female).Count();
        var englishSpeakingCount = totalGuides.Where(g => g.Languages.Any(l => l.NameEN == "English")).Count();
        var polyglotCount = totalGuides.Where(g => g.Languages.Count >= 7).Count();

        var percentage = new Percentage(Math.Round((decimal)maleCount / totalGuidesCount * 100, 2),
            Math.Round((decimal)femaleCount / totalGuidesCount * 100, 2),
            Math.Round((decimal)maleAssignmentCount / totalSchedulesCount * 100, 2),
            Math.Round((decimal)femaleAssignmentCount / totalSchedulesCount * 100, 2),
            Math.Round((decimal)englishSpeakingCount / totalGuidesCount * 100, 2),
            Math.Round((decimal)polyglotCount / totalGuidesCount * 100, 2));
        response.Payload = new GuideStatisticsResponse(totalGuidesCount, totalSchedulesCount, percentage);
        return response;
    }

    public async Task<APIResponse<GuideResponse>> SetOnLeaveAsync(string id, int day)
    {
        var response = new APIResponse<GuideResponse>();

        var currentLanguage = GetCurrentLanguage();

        if (!id.IsValidGuid() || day <= 0)
        {
            var msgInvalidRequest = GetMessageByLocalization(InvalidRequest);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        if (day > int.Parse(_configuration[GuideMaxAllowedOffDays]))
        {
            var msgInvalidRequest = GetMessageByLocalization(ExceedingOffDays);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var guide = await _guideReadRepository.FirstOrDefaultAsync(g => g.Id == Guid.Parse(id) && !g.IsDeleted
            && g.Status == GuideStatus.Active,
            include: g => g.Include(g => g.Languages)
                           .Include(g => g.GuideSchedules)
                           .AsSplitQuery());
        if (guide is null)
        {
            var msgNotFound = GetMessageByLocalization(NotFound);
            response.Message = msgNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgNotFound.state;
            return response;
        }

        var guideLastSchedule = guide.GuideSchedules
            .OrderByDescending(s => s.EndDate)
            .FirstOrDefault();
        var today = DateTime.UtcNow.AddDays(-1);
        if (guideLastSchedule is not null)
        {
            if (today.Date < guideLastSchedule.EndDate.Date)
            {
                var msgOnTour = GetMessageByLocalization(GuideOnTourOrWillBe);
                response.Message = msgOnTour.message;
                response.ResponseCode = HttpStatusCode.BadRequest;
                response.State = msgOnTour.state;
                return response;
            }
        }

        guide.Status = GuideStatus.OnLeave;
        guide.IsAvailable = false;
        _guideWriteRepository.Update(guide);
        await _guideWriteRepository.SaveChangesAsync();

        var scheduler = await _schedulerFactory.GetScheduler();

        var job = JobBuilder.Create<GuideStatusJob>()
            .WithIdentity($"GuideStatusJob-{guide.Id.ToString()}")
            .UsingJobData("GuideId", guide.Id.ToString())
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity($"GuideStatusTrigger-{guide.Id.ToString()}")
            .StartAt(DateTimeOffset.UtcNow.AddDays(day))
            .Build();

        await scheduler.ScheduleJob(job, trigger);

        response.Payload = new GuideResponse(guide.Id,
            guide.FullName,
            guide.Email,
            guide.Gender == Gender.Male ? "Male" : (guide.Gender == Gender.Female ? "Female" : "Other"),
            guide.Languages.Select(l => Helper.GetByLanguage(l.NameEN, l.NameAR, currentLanguage)).ToList(),
            guide.IsAvailable,
            guide.Status == Domain.GuideStatus.Active ? true : false,
            guideLastSchedule is not null ? $"{guideLastSchedule.StartDate.Date.ToUAE().AddDays(1).ToString("yyyy-MM-dd")} - {guideLastSchedule.EndDate.Date.ToUAE().AddDays(1).ToString("yyyy-MM-dd")}" : null,
            guide.GuideSchedules.Where(s => s.StartDate < today && !s.IsDeleted).Count(),
            guide.GuideSchedules.Where(s => s.StartDate >= today && !s.IsDeleted).Count());
        return response;
    }

    public async Task<APIResponse<GuideResponse>> SetRetireAsync(string id)
    {
        var response = new APIResponse<GuideResponse>();

        var currentLanguage = GetCurrentLanguage();

        if (!id.IsValidGuid())
        {
            var msgInvalidRequest = GetMessageByLocalization(InvalidRequest);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var guide = await _guideReadRepository.FirstOrDefaultAsync(g => g.Id == Guid.Parse(id) && !g.IsDeleted
            && g.Status == GuideStatus.Active,
            include: g => g.Include(g => g.Languages)
                           .Include(g => g.GuideSchedules)
                           .AsSplitQuery());
        if (guide is null)
        {
            var msgNotFound = GetMessageByLocalization(NotFound);
            response.Message = msgNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgNotFound.state;
            return response;
        }

        var guideLastSchedule = guide.GuideSchedules
            .OrderByDescending(s => s.EndDate)
            .FirstOrDefault();
        var today = DateTime.UtcNow.AddDays(-1);
        if (guideLastSchedule is not null)
        {
            if (today.Date < guideLastSchedule.EndDate.Date)
            {
                var msgOnTour = GetMessageByLocalization(GuideOnTourOrWillBe);
                response.Message = msgOnTour.message;
                response.ResponseCode = HttpStatusCode.BadRequest;
                response.State = msgOnTour.state;
                return response;
            }
        }

        guide.IsDeleted = true;
        guide.IsAvailable = false;
        guide.Status = GuideStatus.Retired;

        var userId = await GetUserIdByGuideIdAsync(guide.Id.ToString());
        if (userId is null)
        {
            var msgNotFound = GetMessageByLocalization(UserCanNotBeFound);
            response.Message = msgNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgNotFound.state;
            return response;
        }
        _userService.RemoveRoleFromUserAsync(userId, new string[] { "Guide" }).Wait();

        _guideWriteRepository.Update(guide);
        await _guideWriteRepository.SaveChangesAsync();

        response.Payload = new GuideResponse(guide.Id,
            guide.FullName,
            guide.Email,
            guide.Gender == Gender.Male ? "Male" : (guide.Gender == Gender.Female ? "Female" : "Other"),
            guide.Languages.Select(l => Helper.GetByLanguage(l.NameEN, l.NameAR, currentLanguage)).ToList(),
            guide.IsAvailable,
            guide.Status == Domain.GuideStatus.Active ? true : false,
            guideLastSchedule is not null ? $"{guideLastSchedule.StartDate.Date.ToUAE().AddDays(1).ToString("yyyy-MM-dd")} - {guideLastSchedule.EndDate.Date.ToUAE().AddDays(1).ToString("yyyy-MM-dd")}" : null,
            guide.GuideSchedules.Where(s => s.StartDate < today && !s.IsDeleted).Count(),
            guide.GuideSchedules.Where(s => s.StartDate >= today && !s.IsDeleted).Count());
        return response;
    }

    public async Task<APIResponse<GuideResponse>> DeleteAsync(string id)
    {
        var response = new APIResponse<GuideResponse>();

        var currentLanguage = GetCurrentLanguage();

        if (!id.IsValidGuid())
        {
            var msgInvalidRequest = GetMessageByLocalization(InvalidRequest);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var guide = await _guideReadRepository.FirstOrDefaultAsync(g => g.Id == Guid.Parse(id) && !g.IsDeleted
            && g.Status == GuideStatus.Active,
            include: g => g.Include(g => g.Languages)
                           .Include(g => g.GuideSchedules)
                           .AsSplitQuery());
        if (guide is null)
        {
            var msgNotFound = GetMessageByLocalization(NotFound);
            response.Message = msgNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgNotFound.state;
            return response;
        }

        var guideLastSchedule = guide.GuideSchedules
            .OrderByDescending(s => s.EndDate)
            .FirstOrDefault();
        var today = DateTime.UtcNow.AddDays(-1);
        if (guideLastSchedule is not null)
        {
            if (today.Date < guideLastSchedule.EndDate.Date)
            {
                var msgOnTour = GetMessageByLocalization(GuideOnTourOrWillBe);
                response.Message = msgOnTour.message;
                response.ResponseCode = HttpStatusCode.BadRequest;
                response.State = msgOnTour.state;
                return response;
            }
        }

        guide.IsDeleted = true;
        guide.IsAvailable = false;
        guide.Status = GuideStatus.Retired;

        var userId = await GetUserIdByGuideIdAsync(guide.Id.ToString());
        if (userId is null)
        {
            var msgNotFound = GetMessageByLocalization(UserCanNotBeFound);
            response.Message = msgNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgNotFound.state;
            return response;
        }
        _userService.RemoveRoleFromUserAsync(userId, new string[] { "Guide" }).Wait();

        _guideWriteRepository.Update(guide);
        await _guideWriteRepository.SaveChangesAsync();

        response.Payload = new GuideResponse(guide.Id,
            guide.FullName,
            guide.Email,
            guide.Gender == Gender.Male ? "Male" : (guide.Gender == Gender.Female ? "Female" : "Other"),
            guide.Languages.Select(l => Helper.GetByLanguage(l.NameEN, l.NameAR, currentLanguage)).ToList(),
            guide.IsAvailable,
            guide.Status == Domain.GuideStatus.Active ? true : false,
            guideLastSchedule is not null ? $"{guideLastSchedule.StartDate.Date.ToUAE().AddDays(1).ToString("yyyy-MM-dd")} - {guideLastSchedule.EndDate.Date.ToUAE().AddDays(1).ToString("yyyy-MM-dd")}" : null,
            guide.GuideSchedules.Where(s => s.StartDate < today && !s.IsDeleted).Count(),
            guide.GuideSchedules.Where(s => s.StartDate >= today && !s.IsDeleted).Count());
        return response;
    }

    public async Task<APIResponse<GuideResponse>> UpdateAsync(UpdateGuideDTO updateGuideDTO)
    {
        var response = new APIResponse<GuideResponse>();

        var currentLanguage = GetCurrentLanguage();

        var authData = GetAuthData();

        if (string.IsNullOrEmpty(authData.authId) || !authData.authId.IsValidGuid())
        {
            var msgUnauthorized = GetMessageByLocalization(UnauthorizedUser);
            response.ResponseCode = HttpStatusCode.Unauthorized;
            response.Message = msgUnauthorized.message;
            response.State = msgUnauthorized.state;
            return response;
        }

        authData.authId = await _userService.GetGuideIdByUserIdAsync(authData.authId);
        if (string.IsNullOrEmpty(authData.authId) || !authData.authId.IsValidGuid())
        {
            var msgUnauthorized = GetMessageByLocalization(UnauthorizedUser);
            response.ResponseCode = HttpStatusCode.Unauthorized;
            response.Message = msgUnauthorized.message;
            response.State = msgUnauthorized.state;
            return response;
        }

        var guide = await _guideReadRepository.FirstOrDefaultAsync(g => g.Id == Guid.Parse(authData.authId) && !g.IsDeleted
            && g.Status == GuideStatus.Active,
            include: g => g.Include(g => g.Languages)
                           .Include(g => g.GuideSchedules)
                           .AsSplitQuery());
        if (guide is null)
        {
            var msgNotFound = GetMessageByLocalization(NotFound);
            response.Message = msgNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgNotFound.state;
            return response;
        }

        guide.FullName = updateGuideDTO.NewFullName is not null ? updateGuideDTO.NewFullName.ToProperFullName() : guide.FullName;
        var oldEmail = guide.Email;
        guide.Email = updateGuideDTO.NewEmail ?? guide.Email;
        if (updateGuideDTO.NewLanguageIds is not null)
        {
            if(updateGuideDTO.NewLanguageIds.Count > 0)
                await AssignLanguageAsync(guide.Id.ToString(), updateGuideDTO.NewLanguageIds);
        }

        if (updateGuideDTO.NewEmail is not null)
            await _mailService.SendMailAsync(new string[] { guide.Email }, "Informing About Changes ", GetGuideNotificationBody(guide.FullName.ToProperFirstName(), oldEmail, guide.Email));

        _guideWriteRepository.Update(guide);
        await _guideWriteRepository.SaveChangesAsync();

        var guideLastSchedule = guide.GuideSchedules
            .OrderByDescending(s => s.EndDate)
            .FirstOrDefault();

        var today = DateTime.UtcNow.AddDays(-1);
        response.Payload = new GuideResponse(guide.Id,
            guide.FullName,
            guide.Email,
            guide.Gender == Gender.Male ? "Male" : (guide.Gender == Gender.Female ? "Female" : "Other"),
            guide.Languages.Select(l => Helper.GetByLanguage(l.NameEN, l.NameAR, currentLanguage)).ToList(),
            guide.IsAvailable,
            guide.Status == Domain.GuideStatus.Active ? true : false,
            guideLastSchedule is not null ? $"{guideLastSchedule.StartDate.Date.ToUAE().AddDays(1).ToString("yyyy-MM-dd")} - {guideLastSchedule.EndDate.Date.ToUAE().AddDays(1).ToString("yyyy-MM-dd")}" : null,
            guide.GuideSchedules.Where(s => s.StartDate < today && !s.IsDeleted).Count(),
            guide.GuideSchedules.Where(s => s.StartDate >= today && !s.IsDeleted).Count());
        return response;
    }

    public async Task<APIResponse<SetScheduleResponse>> UpdateScheduleAsync(string id, string scheduleId, DateTime newStartDate, DateTime newEndDate)
    {
        var response = new APIResponse<SetScheduleResponse>();

        if (!id.IsValidGuid() || !scheduleId.IsValidGuid())
        {
            var msgInvalidRequest = GetMessageByLocalization(InvalidRequest);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        newStartDate = newStartDate.ToUTC();
        newEndDate = newEndDate.ToUTC();
        var today = DateTime.UtcNow.AddDays(-1);
        if (newStartDate.Date < today.Date || newEndDate.Date < today.Date || newStartDate >= newEndDate)
        {
            var msgInvalidRequest = GetMessageByLocalization(InvalidDate);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var guide = await _guideReadRepository.FirstOrDefaultAsync(g => g.Id == Guid.Parse(id) && !g.IsDeleted
            && g.Status == GuideStatus.Active,
            include: g => g.Include(g => g.GuideSchedules));
        if (guide is null)
        {
            var msgNotFound = GetMessageByLocalization(NotFound);
            response.ResponseCode = HttpStatusCode.NotFound;
            response.Message = msgNotFound.message;
            response.State = msgNotFound.state;
            return response;
        }

        var schedule = guide.GuideSchedules.FirstOrDefault(s => s.Id == Guid.Parse(scheduleId) && !s.IsDeleted);
        if (schedule is null)
        {
            var msgNotFound = GetMessageByLocalization(ScheduleNotFound);
            response.ResponseCode = HttpStatusCode.NotFound;
            response.Message = msgNotFound.message;
            response.State = msgNotFound.state;
            return response;
        }

        bool isOverlapping = guide.GuideSchedules.Any(s => s.Id != Guid.Parse(scheduleId)
            && (s.StartDate <= newEndDate && s.EndDate >= newStartDate) && !s.IsDeleted);
        if (isOverlapping || (schedule.StartDate.Date == newStartDate.Date && schedule.EndDate.Date == newEndDate.Date))
        {
            var msgInvalidRequest = GetMessageByLocalization(AlreadyAssignedInThisPeriod);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var oldStartDate = schedule.StartDate;
        var oldEndDate = schedule.EndDate;

        schedule.StartDate = newStartDate;
        schedule.EndDate = newEndDate;

        var hasActiveSchedule = guide.GuideSchedules
            .Any(s => s.StartDate <= today && s.EndDate >= today && !s.IsDeleted);

        var hasUpcomingSchedule = guide.GuideSchedules.Where(s => s.Id != Guid.Parse(scheduleId))
            .Any(s => s.StartDate.Date <= today.AddDays(int.Parse(_configuration[MaxDaysBetweenSchedules])).Date
                && s.StartDate.Date >= today.Date && !s.IsDeleted);
        if ((newStartDate.Date - today.Date).Days <= int.Parse(_configuration[MaxDaysBetweenSchedules]))
            hasUpcomingSchedule = true;

        guide.IsAvailable = !hasActiveSchedule && !hasUpcomingSchedule;

        _guideWriteRepository.Update(guide);
        await _guideScheduleWriteRepository.SaveChangesAsync();

        await _mailService.SendMailAsync(new string[] { guide.Email }, "Schedule Updated Successfully",
            GetScheduleUpdateEmailBody(guide.FullName, oldStartDate.ToUAE(), oldEndDate.ToUAE(), newStartDate.ToUAE(), newEndDate.ToUAE()));

        response.Payload = new SetScheduleResponse(guide.Id,
            guide.IsAvailable,
            newStartDate.ToUAE().ToString("yyyy-MM-dd"),
            newEndDate.ToUAE().ToString("yyyy-MM-dd"));

        return response;
    }

    public async Task<APIResponse<GuideResponse>> UnassignLanguageAsync(List<string> languageIds)
    {
        var response = new APIResponse<GuideResponse>();

        var currentLanguage = GetCurrentLanguage();

        var authData = GetAuthData();

        if (string.IsNullOrEmpty(authData.authId) || !authData.authId.IsValidGuid())
        {
            var msgUnauthorized = GetMessageByLocalization(UnauthorizedUser);
            response.ResponseCode = HttpStatusCode.Unauthorized;
            response.Message = msgUnauthorized.message;
            response.State = msgUnauthorized.state;
            return response;
        }

        authData.authId = await _userService.GetGuideIdByUserIdAsync(authData.authId);
        if (string.IsNullOrEmpty(authData.authId) || !authData.authId.IsValidGuid())
        {
            var msgUnauthorized = GetMessageByLocalization(UnauthorizedUser);
            response.ResponseCode = HttpStatusCode.Unauthorized;
            response.Message = msgUnauthorized.message;
            response.State = msgUnauthorized.state;
            return response;
        }

        if (languageIds is null || languageIds.Count == 0 || languageIds.Any(l => !l.IsValidGuid()))
        {
            var msgInvalidRequest = GetMessageByLocalization(InvalidRequest);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var guide = await _guideReadRepository.FirstOrDefaultAsync(g => g.Id == Guid.Parse(authData.authId) && !g.IsDeleted
            && g.Status == GuideStatus.Active,
            include: g => g.Include(g => g.Languages)
                           .Include(g => g.GuideSchedules)
                           .AsSplitQuery());
        if (guide is null)
        {
            var msgNotFound = GetMessageByLocalization(NotFound);
            response.ResponseCode = HttpStatusCode.NotFound;
            response.Message = msgNotFound.message;
            response.State = msgNotFound.state;
            return response;
        }

        var guideExistedLanguages = guide.Languages.Select(l => l.Id).ToList();
        if (guideExistedLanguages.Count == int.Parse(_configuration[BasicViolation]) || languageIds.Count == guideExistedLanguages.Count
            || guideExistedLanguages.Count - languageIds.Count < int.Parse(_configuration[BasicViolation]))
        {
            var msgInvalidRequest = GetMessageByLocalization(MustHaveAtLeastTwoLanguages);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        if (languageIds.Any(l => !guideExistedLanguages.Contains(Guid.Parse(l))) || languageIds.Count > guideExistedLanguages.Count)
        {
            var msgInvalidRequest = GetMessageByLocalization(InvalidRequest);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        guide.Languages = guide.Languages.Where(l => !languageIds.Contains(l.Id.ToString())).ToList();
        _guideWriteRepository.Update(guide);
        await _guideWriteRepository.SaveChangesAsync();

        var guideLastSchedule = guide.GuideSchedules
            .OrderByDescending(s => s.EndDate)
            .FirstOrDefault();

        var today = DateTime.UtcNow.AddDays(-1);
        response.Payload = new GuideResponse(guide.Id,
            guide.FullName,
            guide.Email,
            guide.Gender == Gender.Male ? "Male" : (guide.Gender == Gender.Female ? "Female" : "Other"),
            guide.Languages.Select(l => Helper.GetByLanguage(l.NameEN, l.NameAR, currentLanguage)).ToList(),
            guide.IsAvailable,
            guide.Status == Domain.GuideStatus.Active ? true : false,
            guideLastSchedule is not null ? $"{guideLastSchedule.StartDate.Date.ToUAE().AddDays(1).ToString("yyyy-MM-dd")} - {guideLastSchedule.EndDate.Date.ToUAE().AddDays(1).ToString("yyyy-MM-dd")}" : null,
            guide.GuideSchedules.Where(s => s.StartDate < today && !s.IsDeleted).Count(),
            guide.GuideSchedules.Where(s => s.StartDate >= today && !s.IsDeleted).Count());
        return response;
    }

    public async Task<APIResponse<Pagination<GetGuideSchedulesResponse>>> GetGuideSchedulesAsync(int pageNumber = 1, int take = 10, bool isPaginated = false)
    {
        var response = new APIResponse<Pagination<GetGuideSchedulesResponse>>();

        var authData = GetAuthData();

        if (string.IsNullOrEmpty(authData.authId) || !authData.authId.IsValidGuid())
        {
            var msgUnauthorized = GetMessageByLocalization(UnauthorizedUser);
            response.ResponseCode = HttpStatusCode.Unauthorized;
            response.Message = msgUnauthorized.message;
            response.State = msgUnauthorized.state;
            return response;
        }

        if (pageNumber < 1 || take < 1)
        {
            var msgInvalidRequest = GetMessageByLocalization(InvalidRequest);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        authData.authId = await _userService.GetGuideIdByUserIdAsync(authData.authId);
        if (string.IsNullOrEmpty(authData.authId) || !authData.authId.IsValidGuid())
        {
            var msgUnauthorized = GetMessageByLocalization(UnauthorizedUser);
            response.ResponseCode = HttpStatusCode.Unauthorized;
            response.Message = msgUnauthorized.message;
            response.State = msgUnauthorized.state;
            return response;
        }

        var guide = await _guideReadRepository.FirstOrDefaultAsync(g => g.Id == Guid.Parse(authData.authId) && !g.IsDeleted
            && g.Status == GuideStatus.Active,
            include: g => g.Include(g => g.GuideSchedules));
        if (guide is null)
        {
            var msgNotFound = GetMessageByLocalization(NotFound);
            response.Message = msgNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgNotFound.state;
            return response;
        }

        var today = DateTime.UtcNow.AddDays(-1);
        var guideSchedules = isPaginated
            ? guide.GuideSchedules
                .Where(s => s.StartDate >= today && !s.IsDeleted)
                .OrderBy(g => g.StartDate)
                .Skip((pageNumber - 1) * take)
                .Take(take)
            : guide.GuideSchedules
                .Where(s => s.StartDate >= today && !s.IsDeleted)
                .OrderBy(g => g.StartDate);

        var guideSchedulesCount = guideSchedules.Count();
        response.Payload = new Pagination<GetGuideSchedulesResponse>()
        {
            Items = guideSchedules.Select(x => new GetGuideSchedulesResponse(x.Id, x.StartDate.ToUAE(), x.EndDate.ToUAE(), null, null)).ToList(),
            PageIndex = pageNumber,
            PageSize = isPaginated ? take : guideSchedulesCount,
            TotalCount = guideSchedulesCount,
            TotalPage = isPaginated ? (int)Math.Ceiling((double)guideSchedulesCount / take) : 1
        };
        return response;

    }// LOOK AT GuideShcedule

    public async Task<APIResponse<Pagination<GetAllByLanguageResponse>>> GetAllByLanguageAsync(string language, int pageNumber = 1, int take = 10, bool isPaginated = false)
    {
        var response = new APIResponse<Pagination<GetAllByLanguageResponse>>();

        var currentLanguage = GetCurrentLanguage();

        if (pageNumber < 1 || take < 1)
        {
            var msgInvalidRequest = GetMessageByLocalization(InvalidRequest);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var guides = (await (isPaginated
            ? _guideReadRepository.GetAll(g => !g.IsDeleted
                && g.Status == GuideStatus.Active,
                include: g => g.Include(g => g.Languages)
                               .Include(g => g.GuideSchedules)
                               .AsSplitQuery())
                .Skip((pageNumber - 1) * take)
                .Take(take)
            : _guideReadRepository.GetAll(g => !g.IsDeleted
                && g.Status == GuideStatus.Active,
                include: g => g.Include(g => g.Languages)
                               .Include(g => g.GuideSchedules)
                               .AsSplitQuery()))
            .ToListAsync()).Where(g => g.Languages.Any(l => Helper.GetByLanguage(l.NameEN, l.NameAR, currentLanguage).ToLower()
            .Contains(language.ToLower())))
        .ToList();

        var guidesCount = guides.Count();
        var today = DateTime.UtcNow.AddDays(-1);
        response.Payload = new Pagination<GetAllByLanguageResponse>()
        {
            Items = guides.Select(x => new GetAllByLanguageResponse(x.Id,
                x.FullName,
                x.Email,
                x.Gender == Gender.Male ? "Male" : (x.Gender == Gender.Female ? "Female" : "Other"),
                x.IsAvailable,
                x.Status == GuideStatus.Active ? true : false,
                x.Languages.Select(l => new GuideLanguage(l.Id, Helper.GetByLanguage(l.NameEN, l.NameAR, currentLanguage))).ToList(),
                x.GuideSchedules.Where(s => s.StartDate < today && !s.IsDeleted).Count(),
                x.GuideSchedules.Where(s => s.StartDate >= today && !s.IsDeleted).Count())).ToList(),
            PageIndex = pageNumber,
            PageSize = isPaginated ? take : guidesCount,
            TotalCount = guidesCount,
            TotalPage = isPaginated ? (int)Math.Ceiling((double)guidesCount / take) : 1
        };

        return response;
    }

    public async Task<APIResponse<Pagination<GetAllResponse>>> GetAllAsync(int pageNumber = 1, int take = 10, bool isPaginated = false)
    {
        var response = new APIResponse<Pagination<GetAllResponse>>();

        var currentLanguage = GetCurrentLanguage();

        if (pageNumber < 1 || take < 1)
        {
            var msgInvalidRequest = GetMessageByLocalization(InvalidRequest);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var guides = await (isPaginated
            ? _guideReadRepository.GetAll(g => !g.IsDeleted
                && g.Status == GuideStatus.Active,
                include: g => g.Include(g => g.Languages)
                               .Include(g => g.GuideSchedules)
                               .AsSplitQuery())
                .Skip((pageNumber - 1) * take)
                .Take(take)
            : _guideReadRepository.GetAll(g => !g.IsDeleted
                && g.Status == GuideStatus.Active,
                include: g => g.Include(g => g.Languages)
                               .Include(g => g.GuideSchedules)
                               .AsSplitQuery()))
            .ToListAsync();

        var guidesCount = guides.Count();
        var today = DateTime.UtcNow.AddDays(-1);
        response.Payload = new Pagination<GetAllResponse>()
        {
            Items = guides.Select(x => new GetAllResponse(x.Id,
                x.FullName,
                x.Email,
                x.Gender == Gender.Male ? "Male" : (x.Gender == Gender.Female ? "Female" : "Other"),
                x.IsAvailable,
                x.Status == GuideStatus.Active ? true : false,
                x.Languages.Select(l => new GuideLanguage(l.Id, Helper.GetByLanguage(l.NameEN, l.NameAR, currentLanguage))).ToList(),
                x.GuideSchedules.Select(s => new Application.DTOs.Guide.GuideSchedule(s.Id, s.StartDate.ToUAE(), s.EndDate.ToUAE())).ToList(),
                x.GuideSchedules.Where(s => s.StartDate < today && !s.IsDeleted).Count(),
                x.GuideSchedules.Where(s => s.StartDate >= today && !s.IsDeleted).Count())).ToList(),
            PageIndex = pageNumber,
            PageSize = isPaginated ? take : guidesCount,
            TotalCount = guidesCount,
            TotalPage = isPaginated ? (int)Math.Ceiling((double)guidesCount / take) : 1
        };
        return response;
    }

    #region Private Methods

    private async Task<APIResponse<AssignLanguageResponse>> AssignLanguageAsync(string guideId, List<string> languageIds)
    {
        var response = new APIResponse<AssignLanguageResponse>();

        var curentLanguage = GetCurrentLanguage();

        if (!guideId.IsValidGuid())
        {
            var msgInvalidRequest = GetMessageByLocalization(InvalidRequest);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var guide = await _guideReadRepository.FirstOrDefaultAsync(g => g.Id == Guid.Parse(guideId) && !g.IsDeleted
            && g.Status == GuideStatus.Active,
            include: g => g.Include(g => g.Languages));
        if (guide is null)
        {
            var msgNotFound = GetMessageByLocalization(NotFound);
            response.Message = msgNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgNotFound.state;
            return response;
        }

        if (languageIds.Any(l => !l.IsValidGuid()))
        {
            var msgInvalidRequest = GetMessageByLocalization(InvalidRequest);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var languages = await _languageReadRepository.Table.Where(l => languageIds.Contains(l.Id.ToString()) && !l.IsDeleted).ToListAsync();
        if (languages.Count != languageIds.Count)
        {
            var msgNotFound = GetMessageByLocalization(OneOrMoreLanguagesCanNotBeFound);
            response.Message = msgNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgNotFound.state;
            return response;
        }

        var guideExistedLanguages = guide.Languages.Select(l => l.Id).ToList();
        var newLanguages = languages.Where(l => !guideExistedLanguages.Contains(l.Id)).ToList();
        if (newLanguages.Count == 0 || newLanguages is null)
        {
            var msgNotFound = GetMessageByLocalization(ThereAreNoNewLanguages);
            response.Message = msgNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgNotFound.state;
            return response;
        }

        foreach (var language in newLanguages)
            guide.Languages.Add(language);
        _guideWriteRepository.Update(guide);
        await _guideWriteRepository.SaveChangesAsync();

        response.Payload = new AssignLanguageResponse(guide.Id,
            guide.FullName,
            guide.Email,
            guide.Languages.Select(l => Helper.GetByLanguage(l.NameEN, l.NameAR, curentLanguage)).ToList());
        return response;
    }

    private async Task<string?> GetUserIdByGuideIdAsync(string guideId)
    {
        var guide = await _guideReadRepository.FirstOrDefaultAsync(g => g.Id == Guid.Parse(guideId) && !g.IsDeleted, false);
        if (guide is null)
            return null;
        return await _userService.GetUserIdByEmailAsync(guide.Email);
    }

    private string GetScheduleEmailBody(string guideName, DateTime startDate, DateTime endDate)
    {
        return $@"
        <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    color: #333;
                }}
                .container {{
                    max-width: 600px;
                    margin: 0 auto;
                    padding: 20px;
                    border: 1px solid #ddd;
                    border-radius: 5px;
                    background-color: #f9f9f9;
                }}
                .header {{
                    font-size: 20px;
                    font-weight: bold;
                    margin-bottom: 20px;
                    color: #2c3e50;
                }}
                .footer {{
                    margin-top: 20px;
                    font-size: 12px;
                    color: #777;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>Upcoming Tour Itinerary</div>
                <p>Dear {guideName},</p>
                <p>We are pleased to inform you that a new tour schedule has been assigned to you.</p>
                <p><strong>Schedule Details:</strong></p>
                <ul>
                    <li><strong>Start Date:</strong> {startDate:MM-dd-yyyy}</li>
                    <li><strong>End Date:</strong> {endDate:MM-dd-yyyy}</li>
                </ul>
                <p>Please ensure your availability for this period. If you have any questions, feel free to contact our support team.</p>
                <p>Best regards,</p>
                <p><strong>{_configuration["Mail:DisplayName"]}</strong></p>
                <div class='footer'>
                    This is an automated message, please do not reply. For assistance, contact <a href='mailto:support@globaltours.com'>support@globaltours.com</a>.
                </div>
            </div>
        </body>
        </html>";
    }

    private string GetScheduleUpdateEmailBody(string guideName, DateTime oldStartDate, DateTime oldEndDate, DateTime newStartDate, DateTime newEndDate)
    {
        return $@"
        <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background-color: #f4f4f4;
                    padding: 20px;
                }}
                .container {{
                    background-color: #ffffff;
                    padding: 20px;
                    border-radius: 8px;
                    box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                    max-width: 600px;
                    margin: auto;
                }}
                h2 {{
                    color: #333;
                }}
                p {{
                    font-size: 16px;
                    color: #555;
                }}
                .highlight {{
                    font-weight: bold;
                    color: #2c3e50;
                }}
                .footer {{
                    margin-top: 20px;
                    font-size: 14px;
                    color: #888;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <h2>Updating Schedule</h2>
                <p>Dear <span class='highlight'>{guideName}</span>,</p>
                <p>Your schedule has been successfully updated.</p>
                <p>
                    <strong>Previous Start Date:</strong> {oldStartDate:MM-dd-yyyy}<br>
                    <strong>Previous End Date:</strong> {oldEndDate:MM-dd-yyyy}
                </p>
                <p>
                    <strong>New Start Date:</strong> {newStartDate:MM-dd-yyyy}<br>
                    <strong>New End Date:</strong> {newEndDate:MM-dd-yyyy}
                </p>
                <p class='footer'>Best regards,<br>{_configuration["Mail:DisplayName"]}</p>
            </div>
        </body>
        </html>";
    }

    private string GetGuideNotificationBody(string guideName, string oldEmail, string newEmail)
    {
        return $@"
        <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    color: #333;
                }}
                .container {{
                    max-width: 600px;
                    margin: 0 auto;
                    padding: 20px;
                    border: 1px solid #ddd;
                    border-radius: 5px;
                    background-color: #f9f9f9;
                }}
                .header {{
                    font-size: 20px;
                    font-weight: bold;
                    margin-bottom: 20px;
                    color: #2c3e50;
                }}
                .footer {{
                    margin-top: 20px;
                    font-size: 12px;
                    color: #777;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>Guide Information Updated</div>
                <p>Dear {guideName},</p>
                <p>We would like to inform you that your profile information has been updated successfully.</p>
                <p><strong>Updated Details:</strong></p>
                <ul>
                    <li><strong>Previous Email:</strong> {oldEmail}</li>
                    <li><strong>New Email:</strong> {newEmail}</li>
                </ul>
                <p>If you did not request this change, please contact our support team immediately.</p>
                <p>Best regards,</p>
                <p><strong>{_configuration["Mail:DisplayName"]}</strong></p>
                <div class='footer'>
                    This is an automated message, please do not reply. For assistance, contact <a href='mailto:support@globaltours.com'>support@globaltours.com</a>.
                </div>
            </div>
        </body>
        </html>";
    }

    #endregion
}
