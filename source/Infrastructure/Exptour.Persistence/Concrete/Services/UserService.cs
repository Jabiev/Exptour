using Exptour.Application.Abstract.Services;
using Exptour.Application.DTOs.User;
using Exptour.Application.Validators.User;
using Exptour.Common.Helpers;
using Exptour.Common.Infrastructure.Services;
using Exptour.Common.Shared;
using Exptour.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text;

namespace Exptour.Persistence.Concrete.Services;

public class UserService : BaseService, IUserService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    public UserService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, UserManager<ApplicationUser> userManager) : base(httpContextAccessor, configuration)
    {
        _configuration = configuration;
        _userManager = userManager;
    }

    public async Task<APIResponse<Microsoft.AspNetCore.Mvc.EmptyResult>> UpdatePasswordAsync(UpdatePasswordDTO updatePasswordDTO)
    {
        var response = new APIResponse<Microsoft.AspNetCore.Mvc.EmptyResult>();

        UpdatePasswordDTOValidator validations = new();

        var result = await validations.ValidateAsync(updatePasswordDTO);

        if (!result.IsValid)
        {
            StringBuilder stringBuilder = new();
            foreach (var error in result.Errors)
                stringBuilder.AppendLine(error.ErrorMessage);
            response.ResponseCode = HttpStatusCode.UnprocessableContent;
            response.Message = stringBuilder.ToString();
            response.State = GetMessageByLocalization("InvalidRequest").state;
            return response;
        }

        ApplicationUser user = await _userManager.FindByIdAsync(updatePasswordDTO.UserId);
        if (user is null)
        {
            var msgUserNotFound = GetMessageByLocalization("UserDoesNotFound");
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
            response.State = GetMessageByLocalization("Failure").state;
            return response;
        }

        await _userManager.UpdateSecurityStampAsync(user);
        return response;
    }
}
