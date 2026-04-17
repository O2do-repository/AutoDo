using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class AiNormalizationService : IAiNormalizationService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _modelId;
    private readonly string _endpoint;

    public AiNormalizationService(HttpClient httpClient, string apiKey, string modelId, string productId)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
        _modelId = modelId;

        _httpClient.BaseAddress = new Uri("https://api.infomaniak.com/");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        _endpoint = $"1/ai/{productId}/openai/chat/completions";
    }

    public async Task<NormalizedData> NormalizeAsync(
        string jobTitleInput, 
        List<string> rawSkillsInput, 
        List<string>? rawKeywordsInput = null)
    {
        var skillsString = string.Join(" | ", rawSkillsInput.Distinct().Where(s => !string.IsNullOrWhiteSpace(s)));
        var keywordsString = (rawKeywordsInput != null && rawKeywordsInput.Any()) 
            ? string.Join(" | ", rawKeywordsInput.Distinct().Where(k => !string.IsNullOrWhiteSpace(k)))
            : "Aucun";

var prompt = $@"
    ### ROLE
    Tu es un expert RH technique multilingue (FR/NL/EN). Ta tâche est de NORMALISER et FUSIONNER les données en ANGLAIS TECHNIQUE STANDARD.

    ### ENTREES
    - Intitulé de poste : {jobTitleInput}
    - Compétences (Skills) : {skillsString}
    - Mots-clés (Keywords) : {keywordsString}

    ### REGLES STRICTES
    1. Langue : Tout doit être en ANGLAIS.
    2. Fusion : Fusionne les synonymes et traductions en un seul terme standard.
       - Exemple : 'Développeur' + 'Developer' + 'Ontwikkelaar' -> 'Developer'
       - Exemple : 'JS' + 'Javascript' -> 'JavaScript'
       - Exemple : 'C#' -> 'CSharp'
       - Exemple : 'Agile werken' + 'Agile' -> 'Agile'
    3. Distinction :
       - Skills : Technologies, outils, logiciels, frameworks, méthodologies précises (ex: 'Java', 'Jira', 'SAP', 'Terraform', 'Agile').
       - Keywords : Concepts transversaux, soft skills, domaines d'expertise (ex: 'Corporate Law', 'M&A', 'Negotiation', 'Change Management').
       - IGNORER complètement : diplômes ('Master in Law', 'Bachelor'), certifications de langue ('Dutch C1', 'English B2', 'French'), inscriptions professionnelles ('Registered at the Bar', 'BIG registration'), années d'expérience ('5 years experience').
    4. Nettoyage : Ignore 'Must have', 'Should have', 'Nice to have', 'N/A', les phrases longues. Extrais uniquement le nom de la compétence ou du concept.
    5. Format : Termes courts et canoniques uniquement.

    ### SORTIE ATTENDUE
    Retourne UNIQUEMENT un objet JSON valide. Aucun texte avant ou après.
    {{
        ""normalizedJobTitle"": ""Titre standard en anglais"",
        ""normalizedSkills"": [""Skill1"", ""Skill2""],
        ""normalizedKeywords"": [""Keyword1"", ""Keyword2""]
    }}
    ";

        var requestBody = new
        {
            model = _modelId,
            messages = new[]
            {
                new { role = "system", content = "Tu es un assistant JSON strict. Retourne UNIQUEMENT du JSON." },
                new { role = "user", content = prompt }
            },
            temperature = 0.1,
            max_tokens = 800
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(_endpoint, content);
        if (!response.IsSuccessStatusCode)
{
    var errorBody = await response.Content.ReadAsStringAsync();
    throw new Exception($"Erreur Infomaniak {(int)response.StatusCode}: {errorBody}");
}
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseString);
        var rawContent = doc.RootElement.GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        if (string.IsNullOrEmpty(rawContent)) throw new Exception("Réponse IA vide");

        rawContent = rawContent.Replace("```json", "").Replace("```", "").Trim();

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<NormalizedData>(rawContent, options) ?? new NormalizedData();
    }
}