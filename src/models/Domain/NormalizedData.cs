using System.Collections.Generic;



public class NormalizedData
{
    public string NormalizedJobTitle { get; set; } = string.Empty;
    
    public List<string> NormalizedSkills { get; set; } = new();
    
    public List<string> NormalizedKeywords { get; set; } = new();
}
