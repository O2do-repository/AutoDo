public interface ITranslationService
{
    Task<string> TranslateTextAsync(string text, string toLang = "en");
}
