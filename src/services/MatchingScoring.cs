using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

public static class MatchingScoring
{
    // Score basé sur la correspondance entre les intitulés de poste du profil et du RFP
    public static (int score, string feedback) ScoreJobTitleMatch(Profile profile, RFP rfp)
    {
        var profileJobTitle = profile.JobTitle ?? "(non renseigné)";
        var rfpJobTitle = rfp.JobTitle ?? "(non renseigné)";
        
        var introFeedback = $"Intitulé du profil : « {profileJobTitle} »\nIntitulé du RFP : « {rfpJobTitle} »\n";

        if (string.IsNullOrWhiteSpace(profile.JobTitle) || string.IsNullOrWhiteSpace(rfp.JobTitle))
            return (0, introFeedback + "L’intitulé de poste est manquant dans le profil ou le RFP.");

        var rfpWords = TokenizeAndNormalize(rfp.JobTitle);
        if (rfpWords.Count == 0)
            return (0, introFeedback + "Aucun mot significatif trouvé dans l’intitulé du RFP.");

        var profileWords = TokenizeAndNormalize(profile.JobTitle);
        var skillWords = profile.Skills?.SelectMany(TokenizeAndNormalize) ?? Enumerable.Empty<string>();
        var keywordWords = profile.Keywords?.SelectMany(TokenizeAndNormalize) ?? Enumerable.Empty<string>();

        var allProfileWords = profileWords
            .Concat(skillWords)
            .Concat(keywordWords)
            .Distinct()
            .ToList();

        var matchedPairs = rfpWords
            .SelectMany(rfpWord => allProfileWords
                .Where(profileWord => WordsMatch(rfpWord, profileWord))
                .Select(profileWord => (rfpWord, profileWord)))
            .Distinct()
            .ToList();

        int matchCount = matchedPairs.Count;
        double ratio = (double)matchCount / rfpWords.Count;

        int score = ratio switch
        {
            >= 0.8 => 20,
            >= 0.5 => 15,
            >= 0.3 => 10,
            >= 0.1 => 5,
            _ => 0
        };

        string feedback;
        if (matchCount > 0)
        {
            var detailedMatches = string.Join("\n", matchedPairs.Select(p => $"« {p.rfpWord} » (RFP) ↔ « {p.profileWord} » (profil)"));
            feedback = $"{introFeedback}\n{matchCount} mot(s) clé trouvés sur {rfpWords.Count} dans l’intitulé du RFP.\n" +
                    $"Détail des correspondances :\n{detailedMatches}\n" +
                    $"🔎 Taux de correspondance : {(int)(ratio * 100)}%.";
        }
        else
        {
            feedback = $"{introFeedback}\nAucun mot clé de l’intitulé du RFP n’a été retrouvé dans le profil.";
        }

        return (score, feedback);
    }




    // Score basé sur la correspondance du niveau d'expérience
    public static (int score, string feedback) ScoreExperienceMatch(Profile profile, RFP rfp)
    {
        var inferredExperience = InferExperienceFromText(rfp.JobTitle);
        var finalExperience = inferredExperience != Experience.Unspecified
            ? inferredExperience
            : rfp.ExperienceLevel;

        if (finalExperience == Experience.Unspecified)
            return (0, "Niveau d’expérience non identifiable dans le RFP (ni dans l’intitulé, ni explicitement spécifié).");

        if (profile.ExperienceLevel == finalExperience)
            return (20, $"Le profil possède exactement le niveau d’expérience requis : {finalExperience}.");

        return (0, $"Incohérence d’expérience : le RFP demande {finalExperience}, mais le profil indique {profile.ExperienceLevel}.");

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
    public static (int score, string feedback) ScoreSkillsMatch(Profile profile, RFP rfp)
    {
        var rfpSkills = rfp.Skills ?? Enumerable.Empty<string>();

        var profileTokens = profile.Skills?
            .Concat(profile.Keywords ?? Enumerable.Empty<string>())
            .SelectMany(TokenizeAndNormalize)
            .Distinct()
            .ToList() ?? new List<string>();

        int shouldMatched = 0, niceMatched = 0;
        int shouldTotal = 0, niceTotal = 0;
        int unknownMatched = 0, unknownTotal = 0;

        var matchedDetails = new List<string>();
        var unmatchedDetails = new List<string>();

        foreach (var rawSkill in rfpSkills)
        {
            var (text, importance) = ParseSkill(rawSkill);
            var tokens = TokenizeAndNormalize(text);

            bool matched = false;
            foreach (var token in tokens)
            {
                var matchedProfileToken = profileTokens.FirstOrDefault(p => WordsMatch(p, token));
                if (matchedProfileToken != null)
                {
                    matched = true;
                    matchedDetails.Add($"« {text} » (RFP) ↔ « {matchedProfileToken} » (profil)");
                    break;
                }
            }

            if (!matched)
            {
                unmatchedDetails.Add($"« {text} » (RFP) → Pas trouvé dans le profil");
            }

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

        int score;
        if (totalKnown > 0)
        {
            double shouldScore = shouldTotal > 0 ? (double)shouldMatched / shouldTotal * 35 : 0;
            double niceScore = niceTotal > 0 ? (double)niceMatched / niceTotal * 5 : 0;
            score = (int)Math.Round(shouldScore + niceScore);
        }
        else
        {
            double unknownScore = unknownTotal > 0 ? (double)unknownMatched / unknownTotal * 40 : 0;
            score = (int)Math.Round(unknownScore);
        }

        var feedbackBuilder = new StringBuilder();

        feedbackBuilder.AppendLine("Résumé :");
        if (shouldTotal > 0)
            feedbackBuilder.AppendLine($"- Compétences obligatoires (‘must/should have’) : {shouldMatched}/{shouldTotal} trouvées.");
        if (niceTotal > 0)
            feedbackBuilder.AppendLine($"- Compétences facultatives (‘nice to have’) : {niceMatched}/{niceTotal} trouvées.");
        if (unknownTotal > 0)
            feedbackBuilder.AppendLine($"- Compétences sans priorité précisée : {unknownMatched}/{unknownTotal} trouvées.");

        feedbackBuilder.AppendLine();
        feedbackBuilder.AppendLine("Détail des correspondances :");
        if (matchedDetails.Any())
            feedbackBuilder.AppendLine(string.Join("\n", matchedDetails));
        else
            feedbackBuilder.AppendLine("Aucune correspondance détectée.");

        if (unmatchedDetails.Any())
        {
            feedbackBuilder.AppendLine();
            feedbackBuilder.AppendLine("Compétences non trouvées :");
            feedbackBuilder.AppendLine(string.Join("\n", unmatchedDetails));
        }

        return (score, feedbackBuilder.ToString().Trim());
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
    public static (int score, string feedback) ScoreLocationMatch(RFP rfp)
    {
        if (string.IsNullOrWhiteSpace(rfp.Workplace))
            return (0, "Lieu de travail non précisé dans le RFP.");

        var workplaceTokens = TokenizeAndNormalize(rfp.Workplace);

        // Liste des variantes possibles de "Bruxelles"
        var brusselsVariants = new HashSet<string>
        {
            "bruxelles", "brussel", "brussels", "saintjossetennoode", "saintjosse", "regionbruxelloise"
        };

        bool isBrussels = workplaceTokens.Any(t => brusselsVariants.Contains(t));
        if (isBrussels)
        {
            return (20, $"Le lieu de travail spécifié (‘{rfp.Workplace}’) est reconnu comme étant situé dans la région de Bruxelles.");
        }
        else
        {
            return (0, $"Le lieu de travail spécifié (‘{rfp.Workplace}’) est en dehors de Bruxelles ou n’est pas reconnu comme tel.");
        }
    }


    // Tokenise un texte : met en minuscule, enlève les accents, découpe les mots, retire les petits mots
    private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "and", "the", "of", "in", "on", "for", "to", "with", "by", "a", "an",

    };

    private static List<string> TokenizeAndNormalize(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return new();

        // Normalisation Unicode pour enlever les accents
        var normalized = input
            .ToLowerInvariant()
            .Normalize(NormalizationForm.FormD);
        
        var cleaned = new string(normalized
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .ToArray());

        // Utilisation d'une regex pour séparer les mots, tout en gardant les noms techniques comme ".net", "node.js"
        var words = Regex.Matches(cleaned, @"[a-zA-Z0-9#.+]+")
            .Select(m => m.Value)
            .Where(word => word.Length > 2 && !StopWords.Contains(word))
            .Distinct()
            .ToList();

        return words;
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
