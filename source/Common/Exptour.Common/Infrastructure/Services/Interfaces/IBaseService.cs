namespace Exptour.Common.Infrastructure.Services.Interfaces;

public interface IBaseService
{
    string GetHeaderByName(string headerName);
    (string message, string state) GetMessageByLocalization(string key);
    Language GetCurrentLanguage();
}
