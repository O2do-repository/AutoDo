using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;


public class TranslationService : ITranslationService
{
    private readonly AzureTranslatorOptions _options;
    private readonly ILogger<TranslationService> _logger;

    public TranslationService(IOptions<AzureTranslatorOptions> options, ILogger<TranslationService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> TranslateTextAsync(string text, string toLang = "en")
    {
        if (string.IsNullOrWhiteSpace(text)) return text;

        return await RetryService.RetryAsync(async () =>
        {
            var route = $"/translate?api-version=3.0&to={toLang}";
            var body = new object[] { new { Text = text } };
            var requestBody = JsonConvert.SerializeObject(body);

            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, _options.Endpoint + route)
            {
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };

            request.Headers.Add("Ocp-Apim-Subscription-Key", _options.Key);
            request.Headers.Add("Ocp-Apim-Subscription-Region", _options.Region);
            

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            dynamic jsonResponse = JsonConvert.DeserializeObject(result);
            return (string)(jsonResponse[0].translations[0].text ?? text);
        }, logger: _logger);
    }
}
