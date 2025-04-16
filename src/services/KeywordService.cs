public class KeywordService : IKeywordService
{
    private readonly AutoDoDbContext _context;

    public KeywordService(AutoDoDbContext context)
    {
        _context = context;
    }

    // Récupérer tous les mots-clés
    public List<Keyword> GetAllKeywords()
    {
        return _context.Keywords.ToList();
    }

    // Ajouter un nouveau mot-clé
    public Keyword AddKeyword(Keyword keyword)
    {
        keyword.KeywordUuid = Guid.NewGuid();

        _context.Keywords.Add(keyword);
        _context.SaveChanges();

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
}
