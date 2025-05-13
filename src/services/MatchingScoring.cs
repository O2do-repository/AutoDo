using System.Globalization;
using System.Text;

public static class MatchingScoring
{
    // Score basé sur la correspondance entre les intitulés de poste du profil et du RFP
    public static int ScoreJobTitleMatch(Profile profile, RFP rfp)
    {
        // Retourne 0 si l'un des intitulés est vide
        if (string.IsNullOrWhiteSpace(profile.JobTitle) || string.IsNullOrWhiteSpace(rfp.JobTitle))
            return 0;

        // Tokenise et normalise l'intitulé du RFP
        var rfpWords = TokenizeAndNormalize(rfp.JobTitle);
        if (rfpWords.Count == 0) return 0;

        // Tokenise et normalise les infos du profil : poste, compétences, mots-clés
        var profileWords = TokenizeAndNormalize(profile.JobTitle);
        var skillWords = profile.Skills?.SelectMany(TokenizeAndNormalize) ?? Enumerable.Empty<string>();
        var keywordWords = profile.Keywords?.SelectMany(TokenizeAndNormalize) ?? Enumerable.Empty<string>();

        // Regroupe tous les mots uniques issus du profil
        var allProfileWords = profileWords
            .Concat(skillWords)
            .Concat(keywordWords)
            .Distinct()
            .ToList();

        // Compte les mots du RFP qui ont un match avec un mot du profil
        int matchCount = rfpWords.Count(rfpWord =>
            allProfileWords.Any(profileWord => WordsMatch(rfpWord, profileWord)));

        // Calcule le ratio de correspondance
        double ratio = (double)matchCount / rfpWords.Count;

        // Attribue un score selon le ratio
        int score = ratio switch
        {
            >= 0.8 => 20,
            >= 0.5 => 15,
            >= 0.3 => 10,
            >= 0.1 => 5,
            _ => 0
        };

        return score;
    }

    // Score basé sur la correspondance du niveau d'expérience
    public static int ScoreExperienceMatch(Profile profile, RFP rfp)
    {
        // Tente d'inférer l'expérience depuis le titre du RFP
        var inferredExperience = InferExperienceFromText(rfp.JobTitle);

        // Si non inférée, utilise le champ ExperienceLevel du RFP
        var finalExperience = inferredExperience != Experience.Unspecified
            ? inferredExperience
            : rfp.ExperienceLevel;

        // Si l'expérience reste non définie, score 0
        if (finalExperience == Experience.Unspecified)
        {
            return 0;
        }

        // Score de 20 si correspondance exacte avec celle du profil
        return profile.ExperienceLevel == finalExperience ? 20 : 0;
    }

    // Infère le niveau d'expérience à partir du texte
    private static Experience InferExperienceFromText(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return Experience.Unspecified;

        var tokens = TokenizeAndNormalize(text);

        foreach (var token in tokens)
        {
            if (WordsMatch(token, "senior") || WordsMatch(token, "expert") || WordsMatch(token, "sr") || WordsMatch(token, "lead"))
                return Experience.Senior;

            if (WordsMatch(token, "medior") || WordsMatch(token, "confirmé"))
                return Experience.Medior;

            if (WordsMatch(token, "junior") || WordsMatch(token, "jr"))
                return Experience.Junior;
        }

        return Experience.Unspecified;
    }

    // Score basé sur la correspondance des compétences
    public static int ScoreSkillsMatch(Profile profile, RFP rfp)
    {
        
        var rfpSkills = rfp.Skills ?? Enumerable.Empty<string>();

        // Regroupe tous les tokens de compétences et mots-clés du profil
        var profileTokens = profile.Skills?
            .Concat(profile.Keywords ?? Enumerable.Empty<string>())
            .SelectMany(TokenizeAndNormalize)
            .Distinct()
            .ToList() ?? new List<string>();

        int shouldMatched = 0, niceMatched = 0;
        int shouldTotal = 0, niceTotal = 0;
        int unknownMatched = 0, unknownTotal = 0;

        // Parcourt chaque compétence attendue dans le RFP
        foreach (var rawSkill in rfpSkills)
        {
            var (text, importance) = ParseSkill(rawSkill);
            var tokens = TokenizeAndNormalize(text);

            bool matched = tokens.Any(token =>
                profileTokens.Any(p => WordsMatch(p, token)));

            switch (importance)
            {
                case "must have":
                case "should have":
                    shouldTotal++;
                    if (matched) shouldMatched++;
                    break;
                case "nice to have":
                    niceTotal++;
                    if (matched) niceMatched++;
                    break;
                default:
                    unknownTotal++;
                    if (matched) unknownMatched++;
                    break;
            }
        }

        int totalKnown = shouldTotal + niceTotal;
        int matchedKnown = shouldMatched + niceMatched;

        if (totalKnown > 0)
        {
            double shouldScore = shouldTotal > 0 ? (double)shouldMatched / shouldTotal * 35 : 0;
            double niceScore = niceTotal > 0 ? (double)niceMatched / niceTotal * 5 : 0;
            return (int)Math.Round(shouldScore + niceScore);
        }
        else
        {
            double unknownScore = unknownTotal > 0 ? (double)unknownMatched / unknownTotal * 40 : 0;
            return (int)Math.Round(unknownScore);
        }
    }


    // Extrait le texte et l’importance depuis un champ de compétence du RFP
    private static (string text, string importance) ParseSkill(string rawSkill)
    {
        var lower = rawSkill.ToLowerInvariant();
        string importance = "N/A";

        if (lower.Contains("must have")) importance = "must have";
        else if (lower.Contains("should have")) importance = "should have";
        else if (lower.Contains("nice to have")) importance = "nice to have";

        var parts = rawSkill.Split("(importance:", StringSplitOptions.RemoveEmptyEntries);
        string skillText = parts[0].Trim();

        return (skillText, importance);
    }

    // Score basé sur la localisation (actuellement détecte uniquement Bruxelles)
    public static int ScoreLocationMatch(RFP rfp)
    {
        if (string.IsNullOrWhiteSpace(rfp.Workplace))
            return 0;

        var workplaceTokens = TokenizeAndNormalize(rfp.Workplace);

        // Liste des variantes possibles de "Bruxelles"
        var brusselsVariants = new HashSet<string>
        {
            "bruxelles", "brussel", "brussels", "saintjossetennoode", "saintjosse", "regionbruxelloise"
        };

        // Score si l'un des tokens correspond à une variante de Bruxelles
        bool isBrussels = workplaceTokens.Any(t => brusselsVariants.Contains(t));

        return isBrussels ? 20 : 0;
    }

    // Tokenise un texte : met en minuscule, enlève les accents, découpe les mots, retire les petits mots
    private static List<string> TokenizeAndNormalize(string input)
    {
        return input.ToLowerInvariant()
                    .Normalize(NormalizationForm.FormD)
                    .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    .ToArray()
                    .JoinAsString()
                    .Split(new[] { ' ', '-', '_', '/', '\\', ',', ';', '.', '(', ')' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(word => word.Length > 2)
                    .Distinct()
                    .ToList();
    }

    // Compare deux mots avec tolérance à 1 faute de frappe (Levenshtein <= 1)
    private static bool WordsMatch(string word1, string word2)
    {
        if (word1 == word2)
            return true;

        int distance = LevenshteinDistance(word1, word2);
        return distance <= 1 && Math.Min(word1.Length, word2.Length) >= 5;
    }

    // Calcule la distance de Levenshtein entre deux chaînes
    private static int LevenshteinDistance(string a, string b)
    {
        var dp = new int[a.Length + 1, b.Length + 1];
        for (int i = 0; i <= a.Length; i++) dp[i, 0] = i;
        for (int j = 0; j <= b.Length; j++) dp[0, j] = j;

        for (int i = 1; i <= a.Length; i++)
        for (int j = 1; j <= b.Length; j++)
            dp[i, j] = Math.Min(
                Math.Min(dp[i - 1, j] + 1,
                         dp[i, j - 1] + 1), 
                         dp[i - 1, j - 1] + (a[i - 1] == b[j - 1] ? 0 : 1));

        return dp[a.Length, b.Length];
    }

    // Helper pour convertir un tableau de caractères en chaîne
    private static string JoinAsString(this char[] chars)
        => new string(chars);
}
