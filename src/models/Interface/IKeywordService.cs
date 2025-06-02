public interface IKeywordService
{
    List<Keyword> GetAllKeywords();
    Keyword AddKeyword(Keyword keyword);
    void DeleteKeyword(Guid keywordUuid);
}
