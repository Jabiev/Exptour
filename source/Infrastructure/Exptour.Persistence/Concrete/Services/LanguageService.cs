using AutoMapper;
using Exptour.Application.Abstract.Repositories.Languages;
using Exptour.Application.Abstract.Services;
using Exptour.Application.DTOs.Language;
using Exptour.Application.Validators.Language;
using Exptour.Common.Helpers;
using Exptour.Common.Infrastructure.Services;
using Exptour.Common.Shared;
using Exptour.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Net;
using System.Text;
using static Exptour.Application.Constants.ExceptionMessages;

namespace Exptour.Persistence.Concrete.Services;

public class LanguageService : BaseService, ILanguageService
{
    private readonly IMapper _mapper;
    private readonly ILanguageReadRepository _languageReadRepository;
    private readonly ILanguageWriteRepository _languageWriteRepository;

    public LanguageService(IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration,
        IMapper mapper,
        ILanguageReadRepository languageReadRepository,
        ILanguageWriteRepository languageWriteRepository) : base(httpContextAccessor, configuration)
    {
        _mapper = mapper;
        _languageReadRepository = languageReadRepository;
        _languageWriteRepository = languageWriteRepository;
    }

    public async Task<APIResponse<LanguageResponse>> CreateAsync(CreateLanguageDTO createLanguageDto)
    {
        var response = new APIResponse<LanguageResponse>();

        var currentLanguage = GetCurrentLanguage();

        CreateLanguageDTOValidator validations = new();

        var result = await validations.ValidateAsync(createLanguageDto);
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

        if (await _languageReadRepository.FirstOrDefaultAsync(l => l.NameEN.ToLower() == createLanguageDto.NameEN.ToLower() && !l.IsDeleted) is not null)
        {
            var msgLanguageExists = GetMessageByLocalization(AlreadyExists);
            response.Message = msgLanguageExists.message;
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.State = msgLanguageExists.state;
            return response;
        }

        Language language = new()
        {
            NameEN = createLanguageDto.NameEN,
            NameAR = createLanguageDto.NameAR,
            Code = GetIso639Code(createLanguageDto.NameEN)
        };
        await _languageWriteRepository.AddAsync(language);
        await _languageWriteRepository.SaveChangesAsync();

        response.Payload = new LanguageResponse(language.Id, Helper.GetByLanguage(language.NameEN, language.NameAR, currentLanguage), language.Code);
        return response;

    }

    public async Task<APIResponse<LanguageResponse>> DeleteAsync(string id)
    {
        var response = new APIResponse<LanguageResponse>();

        var currentLanguage = GetCurrentLanguage();

        if (!id.IsValidGuid())
        {
            var msgInvalidRequest = GetMessageByLocalization(InvalidRequest);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var language = await _languageReadRepository.FirstOrDefaultAsync(l => l.Id == Guid.Parse(id) && !l.IsDeleted);
        if (language is null)
        {
            var msgNotFound = GetMessageByLocalization(NotFound);
            response.Message = msgNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgNotFound.state;
            return response;
        }

        _languageWriteRepository.Remove(language);
        await _languageWriteRepository.SaveChangesAsync();

        response.Payload = new LanguageResponse(language.Id, Helper.GetByLanguage(language.NameEN, language.NameAR, currentLanguage), language.Code);
        return response;
    }

    public async Task<APIResponse<Pagination<LanguageResponse>>> GetAllAsync(int pageNumber = 1, int take = 10, bool isPaginated = false)
    {
        var response = new APIResponse<Pagination<LanguageResponse>>();

        var currentLanguage = GetCurrentLanguage();

        if (pageNumber < 1 || take < 1)
        {
            var msgInvalidRequest = GetMessageByLocalization(InvalidRequest);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var languages = isPaginated
            ? _languageReadRepository.Where(c => !c.IsDeleted)
                .Skip((pageNumber - 1) * take)
                .Take(take)
            : _languageReadRepository.Where(c => !c.IsDeleted);

        var languagesCount = await languages.CountAsync();

        response.Payload = new Pagination<LanguageResponse>()
        {
            Items = await languages.Select(x => new LanguageResponse(x.Id, Helper.GetByLanguage(x.NameEN, x.NameAR, currentLanguage), x.Code)).ToListAsync(),
            PageIndex = pageNumber,
            PageSize = isPaginated ? take : languagesCount,
            TotalCount = languagesCount,
            TotalPage = isPaginated ? (int)Math.Ceiling((double)languagesCount / take) : 1
        };
        return response;
    }

    public async Task<APIResponse<LanguageResponse>> GetByIdAsync(string id)
    {
        var response = new APIResponse<LanguageResponse>();

        var currentLanguage = GetCurrentLanguage();

        if (!id.IsValidGuid())
        {
            var msgInvalidRequest = GetMessageByLocalization(InvalidRequest);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var language = await _languageReadRepository.FirstOrDefaultAsync(l => l.Id == Guid.Parse(id) && !l.IsDeleted);
        if (language is null)
        {
            var msgNotFound = GetMessageByLocalization(NotFound);
            response.Message = msgNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgNotFound.state;
            return response;
        }

        response.Payload = new LanguageResponse(language.Id, Helper.GetByLanguage(language.NameEN, language.NameAR, currentLanguage), language.Code);
        return response;
    }

    #region Private Methods

    private string GetIso639Code(string languageName)
    {
        var culture = CultureInfo.GetCultures(CultureTypes.NeutralCultures)
            .FirstOrDefault(c => c.EnglishName.Equals(languageName, StringComparison.OrdinalIgnoreCase));

        return culture?.TwoLetterISOLanguageName ?? "N/A";
    }

    #endregion
}
