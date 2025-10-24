public class KeywordService : IKeywordService
{
    private readonly AutoDoDbContext _context;
    private readonly ITranslationService _translationService;

    public KeywordService(AutoDoDbContext context, ITranslationService translationService)
    {
        _context = context;
        _translationService = translationService;

    }

    // Récupérer tous les mots-clés
    public List<Keyword> GetAllKeywords()
    {
        return _context.Keywords.ToList();
    }

    // Ajouter un nouveau mot-clé
    public async Task<Keyword> AddKeyword(Keyword keyword)
    {
        var existingKeyword = _context.Keywords
            .SingleOrDefault(k => k.Name.ToLower() == keyword.Name.ToLower());

        if (existingKeyword != null)
        {
            throw new InvalidOperationException($"Le mot-clé '{keyword.Name}' existe déjà.");
        }

        keyword.KeywordUuid = Guid.NewGuid();

        // Nom d'origine
        var originalName = keyword.Name;

        // Traductions automatiques
        try
        {
            keyword.NameEn = await _translationService.TranslateTextAsync(originalName, "en");
            keyword.NameFr = await _translationService.TranslateTextAsync(originalName, "fr");
            keyword.NameNl = await _translationService.TranslateTextAsync(originalName, "nl");
        }
        catch (Exception)
        {
            keyword.NameEn = originalName;
            keyword.NameFr = originalName;
            keyword.NameNl = originalName;
        }

        _context.Keywords.Add(keyword);
        await _context.SaveChangesAsync();

        return keyword;
    }


    // Supprimer un mot-clé
    public void DeleteKeyword(Guid keywordUuid)
    {
        var existingKeyword = _context.Keywords
            .SingleOrDefault(k => k.KeywordUuid == keywordUuid);

        if (existingKeyword == null)
        {
            throw new Exception($"Le mot-clé avec UUID {keywordUuid} n'existe pas.");
        }

        _context.Keywords.Remove(existingKeyword);
        _context.SaveChanges();
    }
 public async Task TranslateAllKeywords()
    {
        var keyxords = _context.Keywords.ToList();

        foreach (var keyword in keyxords)
        {
            var originalName = keyword.Name;

            try
            {
                keyword.NameEn = await _translationService.TranslateTextAsync(originalName, "en");
                keyword.NameFr = await _translationService.TranslateTextAsync(originalName, "fr");
                keyword.NameNl = await _translationService.TranslateTextAsync(originalName, "nl");
            }
            catch (Exception ex)
            {
                keyword.NameEn = originalName;
                keyword.NameFr = originalName;
                keyword.NameNl = originalName;
            }
        }

        await _context.SaveChangesAsync();
    }

}

