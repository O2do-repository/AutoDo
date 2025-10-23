public interface IKeywordService
{
    List<Keyword> GetAllKeywords();
    Task<Keyword> AddKeyword(Keyword keyword);
    void DeleteKeyword(Guid keywordUuid);
    Task TranslateAllKeywords();

}
