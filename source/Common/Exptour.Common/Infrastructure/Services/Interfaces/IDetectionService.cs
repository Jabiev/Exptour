namespace Exptour.Common.Infrastructure.Services.Interfaces;

public interface IDetectionService
{
    //DetectLanguageAsync
    Task<string?> DetectLanguageAsync(string text);
}
