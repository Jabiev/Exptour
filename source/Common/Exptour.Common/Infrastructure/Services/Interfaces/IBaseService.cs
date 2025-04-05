namespace Exptour.Common.Infrastructure.Services.Interfaces;

public interface IBaseService
{
    string GetHeaderByName(string headerName);
    string? GetAuthToken();
    (string authId, string authRole) GetAuthData();
    (string message, string state) GetMessageByLocalization(string key);
    Language GetCurrentLanguage();
}
