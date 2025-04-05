using DetectLanguage;
using Exptour.Application.Settings;
using Exptour.Common.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Exptour.Infrastructure.Services;

public class DetectionService : IDetectionService
{
    private readonly DetectLanguageAPI _detectLanguageAPISettings;
    private readonly string _apiKey;
    private readonly string _apiUrl;

    public DetectionService(IOptions<DetectLanguageAPI> detectLanguageAPISettings)
    {
        _detectLanguageAPISettings = detectLanguageAPISettings.Value;
        _apiKey = _detectLanguageAPISettings.ApiKey;
        _apiUrl = _detectLanguageAPISettings.ApiUrl;
    }

    public async Task<string?> DetectLanguageAsync(string text)
    {
        //var client = new RestClient(_apiUrl);
        //var request = new RestRequest(Method.Post.ToString());

        //request.AddHeader("Authorization", $"Bearer {_apiKey}");
        //request.AddJsonBody(new { q = text });

        //var response = await client.ExecuteAsync(request);
        //if (response is not null && response.IsSuccessful && response.Content is not null)
        //{
        //    dynamic? result = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content);
        //    var language = result?.data.detections[0].language;
        //    return language;
        //}
        //else
        //    return null;

        DetectLanguageClient client = new DetectLanguageClient(_apiKey);

        return await client.DetectCodeAsync(text);
    }
}