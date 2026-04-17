using System.Collections.Generic;
using System.Threading.Tasks;

public interface IAiNormalizationService
{

    Task<NormalizedData> NormalizeAsync(string jobTitleInput, List<string> rawSkillsInput, List<string>? rawKeywordsInput = null);
}