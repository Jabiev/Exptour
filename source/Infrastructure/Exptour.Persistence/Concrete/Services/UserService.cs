using Exptour.Application.Abstract.Repositories.Endpoints;
using Exptour.Application.Abstract.Repositories.Guides;
using Exptour.Application.Abstract.Services;
using Exptour.Application.DTOs.User;
using Exptour.Application.Validators.User;
using Exptour.Common.Helpers;
using Exptour.Common.Infrastructure.Services;
using Exptour.Common.Infrastructure.Services.Interfaces;
using Exptour.Common.Shared;
using Exptour.Domain.Entities;
using Exptour.Domain.Events;
using Exptour.Infrastructure.ElasticSearch.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Net;
using System.Text;
using static Exptour.Application.Constants.ExceptionMessages;

namespace Exptour.Persistence.Concrete.Services;

public class UserService : BaseService, IUserService
{
    private readonly IConfiguration _configuration;
    private readonly IMediator _mediator;
    private readonly IMessageQueueService _messageQueueService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IEndpointReadRepository _endpointReadRepository;
    private readonly IMailService _mailService;
    private readonly IUserSearchService _userSearchService;
    private readonly IGuideReadRepository _guideReadRepository;
    private readonly IDetectionService _detectionService;

    public UserService(IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration,
        IMediator mediator,
        IMessageQueueService messageQueueService,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IEndpointReadRepository endpointReadRepository,
        IMailService mailService,
        IUserSearchService userSearchService,
        IGuideReadRepository guideReadRepository,
        IDetectionService detectionService) : base(httpContextAccessor, configuration)
    {
        _configuration = configuration;
        _mediator = mediator;
        _messageQueueService = messageQueueService;
        _userManager = userManager;
        _roleManager = roleManager;
        _endpointReadRepository = endpointReadRepository;
        _mailService = mailService;
        _userSearchService = userSearchService;
        _guideReadRepository = guideReadRepository;
        _detectionService = detectionService;
    }

    public async Task<APIResponse<RegisterResponse?>> RegisterAsync(RegisterDTO registerDTO)
    {
        var response = new APIResponse<RegisterResponse?>();

        RegisterDTOValidator validations = new();

        var result = await validations.ValidateAsync(registerDTO);

        if (!result.IsValid)
        {
            StringBuilder stringBuilder = new();
            foreach (var error in result.Errors)
                stringBuilder.AppendLine(error.ErrorMessage);
            response.ResponseCode = HttpStatusCode.UnprocessableContent;
            response.Message = stringBuilder.ToString();
            response.State = GetMessageByLocalization(InvalidRequest).state;
            return response;
        }

        ApplicationUser appUser = new()
        {
            LastName = registerDTO.LastName ?? string.Empty,
            UserName = registerDTO.UserName,
            Email = registerDTO.Email,
            Gender = registerDTO.Gender ?? Domain.Gender.RatherNotSay,
            IsActive = true
        };

        var identityResult = await _userManager.CreateAsync(appUser, registerDTO.Password);

        if (!identityResult.Succeeded)
        {
            StringBuilder stringBuilder = new();
            foreach (var error in identityResult.Errors)
                stringBuilder.AppendLine(error.Description);
            response.ResponseCode = HttpStatusCode.UnprocessableContent;
            response.Message = stringBuilder.ToString();
            response.State = GetMessageByLocalization(Failure).state;
            return response;
        }

        var @event = new UserEvent(appUser.Id, appUser.Email, appUser.FullName, new List<string>());
        await _mediator.Publish(@event);

        await _mailService.SendMailAsync(new string[] { registerDTO.Email }, $"Welcome to {_configuration["Mail:DisplayName"]}", @$"Dear {appUser.UserName},
        We are excited to welcome you to {_configuration["Mail:DisplayName"]}!
        Your registration was successful, and you can now start exploring our platform.
        If you have any questions or need assistance, feel free to contact our support team.
        <br>Best Regards...<br><br><br> {_configuration["Mail:DisplayName"]} Team");

        response.Payload = new RegisterResponse(appUser.Id, appUser.UserName, appUser.Email);
        return response;
    }

    public async Task<APIResponse<Microsoft.AspNetCore.Mvc.EmptyResult>> PasswordResetAsnyc(PasswordResetDTO passwordResetDTO)
    {
        var response = new APIResponse<Microsoft.AspNetCore.Mvc.EmptyResult>();

        PasswordResetDTOValidator validations = new();

        var result = await validations.ValidateAsync(passwordResetDTO);

        if (!result.IsValid)
        {
            StringBuilder stringBuilder = new();
            foreach (var error in result.Errors)
                stringBuilder.AppendLine(error.ErrorMessage);
            response.ResponseCode = HttpStatusCode.UnprocessableContent;
            response.Message = stringBuilder.ToString();
            response.State = GetMessageByLocalization(InvalidRequest).state;
            return response;
        }

        ApplicationUser user = await _userManager.FindByEmailAsync(passwordResetDTO.Email);

        if (user is null)
        {
            var msgUserNotFound = GetMessageByLocalization(UserCanNotBeFound);
            response.Message = msgUserNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgUserNotFound.state;
            return response;
        }

        string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        resetToken = resetToken.UrlEncode();

        await _mailService.SendPasswordResetMailAsync(passwordResetDTO.Email, user.Id.ToString(), resetToken);

        return new APIResponse<Microsoft.AspNetCore.Mvc.EmptyResult>();
    }

    public async Task<APIResponse<bool>> VerifyResetTokenAsync(VerifyResetTokenDto verifyResetTokenDto)
    {
        var response = new APIResponse<bool>();

        VerifyResetTokenDTOValidator validations = new();

        var result = await validations.ValidateAsync(verifyResetTokenDto);

        if (!result.IsValid)
        {
            StringBuilder stringBuilder = new();
            foreach (var error in result.Errors)
                stringBuilder.AppendLine(error.ErrorMessage);
            response.ResponseCode = HttpStatusCode.UnprocessableContent;
            response.Message = stringBuilder.ToString();
            response.State = GetMessageByLocalization(InvalidRequest).state;
            return response;
        }

        ApplicationUser user = await _userManager.FindByIdAsync(verifyResetTokenDto.UserId);
        if (user is null)
        {
            var msgUserNotFound = GetMessageByLocalization(UserCanNotBeFound);
            response.Message = msgUserNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgUserNotFound.state;
            response.Payload = false;
            return response;
        }

        string resetToken = verifyResetTokenDto.ResetToken.UrlDecode();

        response.Payload = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", resetToken);
        if (!response.Payload)
        {
            var msgInvalidResetToken = GetMessageByLocalization(InvalidResetToken);
            response.Message = msgInvalidResetToken.message;
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.State = msgInvalidResetToken.state;
            return response;
        }

        response.ResponseCode = HttpStatusCode.OK;
        return response;
    }

    public async Task<APIResponse<Microsoft.AspNetCore.Mvc.EmptyResult>> UpdatePasswordAsync(string id, UpdatePasswordDTO updatePasswordDTO)
    {
        var response = new APIResponse<Microsoft.AspNetCore.Mvc.EmptyResult>();

        UpdatePasswordDTOValidator validations = new();

        var result = await validations.ValidateAsync(updatePasswordDTO);

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

        ApplicationUser? user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            var msgUserNotFound = GetMessageByLocalization(UserCanNotBeFound);
            response.Message = msgUserNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgUserNotFound.state;
            return response;
        }

        string resetToken = updatePasswordDTO.ResetToken.UrlDecode();
        IdentityResult res = await _userManager.ResetPasswordAsync(user, resetToken, updatePasswordDTO.NewPassword);
        if (!res.Succeeded)
        {
            StringBuilder stringBuilder = new();
            foreach (var error in res.Errors)
                stringBuilder.AppendLine(error.Description);
            response.ResponseCode = HttpStatusCode.UnprocessableContent;
            response.Message = stringBuilder.ToString();
            response.State = GetMessageByLocalization(Failure).state;
            return response;
        }

        await _userManager.UpdateSecurityStampAsync(user);
        return response;
    }

    public async Task<APIResponse<UpdateProfileResponse>> UpdateProfileAsync(UpdateProfileDTO updateProfileDTO)
    {
        var response = new APIResponse<UpdateProfileResponse>();

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

        ApplicationUser? user = await _userManager.FindByIdAsync(authData.authId);
        if (user is null)
        {
            var msgUserNotFound = GetMessageByLocalization(UserCanNotBeFound);
            response.Message = msgUserNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgUserNotFound.state;
            return response;
        }

        user.UserName = updateProfileDTO.UserName ?? user.UserName;
        user.LastName = updateProfileDTO.LastName ?? user.LastName;
        user.PhoneNumber = updateProfileDTO.PhoneNumber ?? user.PhoneNumber;
        user.Email = updateProfileDTO.Email.IsValidEmail() ? updateProfileDTO.Email : user.Email;
        if (updateProfileDTO.Nationality is not null)
        {
            var detectedLanguage = await _detectionService.DetectLanguageAsync(updateProfileDTO.Nationality);

            if (detectedLanguage is "en" || detectedLanguage is "ar")
            {
                if (detectedLanguage is "en")
                {
                    user.NationalityAR = updateProfileDTO.Nationality.GetNationalityAr() ?? user.NationalityAR;
                    user.NationalityEN = user.NationalityAR?.GetNationalityEn() ?? user.NationalityEN;
                }
                else
                {
                    user.NationalityEN = updateProfileDTO.Nationality.GetNationalityEn() ?? user.NationalityEN;
                    user.NationalityAR = user.NationalityEN?.GetNationalityAr() ?? user.NationalityAR;
                }
            }
        }

        IdentityResult result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            StringBuilder stringBuilder = new();
            foreach (var error in result.Errors)
                stringBuilder.AppendLine(error.Description);
            response.ResponseCode = HttpStatusCode.UnprocessableContent;
            response.Message = stringBuilder.ToString();
            response.State = GetMessageByLocalization(Failure).state;
            return response;
        }

        var roles = (await _userManager.GetRolesAsync(user)).ToList();

        var @event = new UserEvent(user.Id, user.Email, user.FullName, roles);
        await _mediator.Publish(@event);

        response.Payload = new UpdateProfileResponse(authData.authId,
            user.UserName,
            user.LastName,
            user.PhoneNumber,
            user.Email,
            Helper.GetByLanguage(user.NationalityEN, user.NationalityAR, currentLanguage));
        return response;
    }

    public async Task<APIResponse<Pagination<UserResponse>>> GetAllUsersAsync(int pageNumber = 1, int take = 10, bool isPaginated = false)
    {
        var response = new APIResponse<Pagination<UserResponse>>();

        if (pageNumber < 1 || take < 1)
        {
            var msgInvalidRequest = GetMessageByLocalization(InvalidRequest);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var users = await _userSearchService.GetAllUsersAsync(isPaginated, pageNumber, take);

        var usersCount = users.Count();

        response.Payload = new Pagination<UserResponse>()
        {
            Items = users,
            PageIndex = pageNumber,
            PageSize = isPaginated ? take : usersCount,
            TotalCount = usersCount,
            TotalPage = isPaginated ? (int)Math.Ceiling((double)usersCount / take) : 1
        };
        return response;
    }

    public async Task<APIResponse<Microsoft.AspNetCore.Mvc.EmptyResult>> AssignRoleToUserAsnyc(AssignRoleDTO assignRoleDTO)
    {
        var response = new APIResponse<Microsoft.AspNetCore.Mvc.EmptyResult>();

        AssignRoleDTOValidator validations = new();

        var res = await validations.ValidateAsync(assignRoleDTO);

        if (!res.IsValid)
        {
            StringBuilder stringBuilder = new();
            foreach (var error in res.Errors)
                stringBuilder.AppendLine(error.ErrorMessage);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = stringBuilder.ToString();
            response.State = GetMessageByLocalization(InvalidRequest).state;
            return response;
        }

        ApplicationUser? user = await _userManager.FindByIdAsync(assignRoleDTO.UserId);
        if (user is null)
        {
            var msgUserNotFound = GetMessageByLocalization(UserCanNotBeFound);
            response.Message = msgUserNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgUserNotFound.state;
            return response;
        }

        var existingRoles = (await _roleManager.Roles.ToListAsync())
            .Select(r => r.Name).ToHashSet();
        var invalidRoles = assignRoleDTO.Roles.Except(existingRoles, StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (invalidRoles.Any())
        {
            var msgRoleNotFound = GetMessageByLocalization(RoleDoesNotExist);
            response.Message = msgRoleNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgRoleNotFound.state;
            return response;
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        var rolesToAdd = assignRoleDTO.Roles.Except(userRoles);
        if (!rolesToAdd.Any())
        {
            var msgNoNewRoles = GetMessageByLocalization(NoNewRolesToAssign);
            response.Message = msgNoNewRoles.message;
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.State = msgNoNewRoles.state;
            return response;
        }

        var result = await _userManager.AddToRolesAsync(user, rolesToAdd);

        if (!result.Succeeded)
        {
            StringBuilder stringBuilder = new();
            foreach (var error in result.Errors)
                stringBuilder.AppendLine(error.Description);
            response.ResponseCode = HttpStatusCode.UnprocessableContent;
            response.Message = stringBuilder.ToString();
            response.State = GetMessageByLocalization(Failure).state;
            return response;
        }

        if (rolesToAdd.Any(r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase)))
        {
            user.IsAdmin = true;
            await _userManager.UpdateAsync(user);
        }

        var roles = (await _userManager.GetRolesAsync(user)).ToList();
        var @event = new UserEvent(user.Id, user.Email, user.FullName, roles);
        await _mediator.Publish(@event);

        return response;
    }

    public async Task<APIResponse<Microsoft.AspNetCore.Mvc.EmptyResult>> RemoveRoleFromUserAsync(string userId, string[] roles)
    {
        var response = new APIResponse<Microsoft.AspNetCore.Mvc.EmptyResult>();

        if (string.IsNullOrEmpty(userId) || !userId.IsValidGuid())
        {
            var msgInvalidRequest = GetMessageByLocalization(InvalidRequest);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        ApplicationUser user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            var msgUserNotFound = GetMessageByLocalization(UserCanNotBeFound);
            response.Message = msgUserNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgUserNotFound.state;
            return response;
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        if (!userRoles.Any())
        {
            var msgNoRolesAssigned = GetMessageByLocalization(NoRolesAssigned);
            response.Message = msgNoRolesAssigned.message;
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.State = msgNoRolesAssigned.state;
            return response;
        }

        var rolesToRemove = roles.Intersect(userRoles);
        if (!rolesToRemove.Any())
        {
            var msgRolesNotFound = GetMessageByLocalization(NotFound);
            response.Message = msgRolesNotFound.message;
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.State = msgRolesNotFound.state;
            return response;
        }

        var result = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

        if (!result.Succeeded)
        {
            StringBuilder stringBuilder = new();
            foreach (var error in result.Errors)
                stringBuilder.AppendLine(error.Description);
            response.ResponseCode = HttpStatusCode.UnprocessableContent;
            response.Message = stringBuilder.ToString();
            response.State = GetMessageByLocalization(Failure).state;
            return response;
        }

        var rolesAfterRemoval = (await _userManager.GetRolesAsync(user)).ToList();
        var @event = new UserEvent(user.Id, user.Email, user.FullName, rolesAfterRemoval);
        await _mediator.Publish(@event);

        return response;
    }

    public async Task<APIResponse<List<string>>> GetRolesAccordingToUserAsync(string userIdOrName)
    {
        var response = new APIResponse<List<string>>();

        ApplicationUser user = null;

        if (userIdOrName.IsValidGuid())
            user = await _userManager.FindByIdAsync(userIdOrName);

        if (user is null)
            user = await _userManager.FindByNameAsync(userIdOrName);


        if (user is null)
        {
            var msgUserNotFound = GetMessageByLocalization(UserCanNotBeFound);
            response.ResponseCode = HttpStatusCode.NotFound;
            response.Message = msgUserNotFound.message;
            response.State = msgUserNotFound.state;
            return response;
        }

        response.Payload = (await _userManager.GetRolesAsync(user)).ToList();
        return response;
    }

    public async Task<APIResponse<Pagination<UserResponse>>> SearchAsync(string search, int pageNumber = 1, int take = 10, bool isPaginated = false)
    {
        var response = new APIResponse<Pagination<UserResponse>>();

        if (string.IsNullOrEmpty(search) || pageNumber < 1 || take < 1)
        {
            var msgInvalidRequest = GetMessageByLocalization(InvalidRequest);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var users = await _userSearchService.SearchUsersAsync(search, isPaginated, pageNumber, take);

        var usersCount = users.Count();
        response.Payload = new Pagination<UserResponse>()
        {
            Items = users,
            PageIndex = pageNumber,
            PageSize = isPaginated ? take : usersCount,
            TotalCount = usersCount,
            TotalPage = isPaginated ? (int)Math.Ceiling((double)usersCount / take) : 1
        };
        return response;
    }

    public async Task<APIResponse<bool>> HasRolePermissionToEndpointAsync(string name, string code)
    {
        var response = new APIResponse<bool>();

        var userRoles = await GetRolesAccordingToUserAsync(name);

        if (userRoles.Payload is null || !userRoles.Payload.Any())
        {
            var msgUserNotFound = GetMessageByLocalization(NoRolesAssignedToUser);
            response.ResponseCode = HttpStatusCode.NotFound;
            response.Message = msgUserNotFound.message;
            response.State = msgUserNotFound.state;
            return response;
        }

        Endpoint? endpoint = await _endpointReadRepository.Table
            .Include(e => e.Menu)
            .Include(e => e.EndpointRoles)
                .ThenInclude(er => er.Role)
                .FirstOrDefaultAsync(e => e.Code == code);

        if (endpoint is null)
        {
            var msgEndpointNotFound = GetMessageByLocalization(NotFound);
            response.Message = msgEndpointNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgEndpointNotFound.state;
            response.Payload = false;
            return response;
        }

        var endpointRoles = endpoint.EndpointRoles.Select(er => er.Role.Name);

        response.Payload = userRoles.Payload.Any(userRole => endpointRoles.Contains(userRole));
        return response;
    }

    public async Task<string?> GetGuideIdByUserIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return (await _guideReadRepository.FirstOrDefaultAsync(g => g.Email == user.Email))?.Id.ToString();
    }

    public async Task<string?> GetUserIdByEmailAsync(string email)
        => (await _userManager.FindByEmailAsync(email))?.Id.ToString();
}