using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public static class MatchingScoring
{
    // Configuration des poids maximaux
    private const int MaxJobTitleScore = 25;
    private const int MaxSkillsScore = 45;
    private const int MaxLocationScore = 10;
    private const int MaxExperienceScore = 20;

    public static (int score, string feedback) ScoreJobTitleMatch(Profile profile, RFP rfp, NormalizedData normProfile = null, NormalizedData normRfp = null)
    {
        if (normProfile != null && normRfp != null &&
            !string.IsNullOrEmpty(normProfile.NormalizedJobTitle) &&
            !string.IsNullOrEmpty(normRfp.NormalizedJobTitle))
        {
            string pTitle = normProfile.NormalizedJobTitle;
            string rTitle = normRfp.NormalizedJobTitle;

            var pWords = pTitle.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var rWords = rTitle.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            double totalScore = rWords.Sum(rw =>
                pWords.Max(pw => NormalizedMatchScore(rw, pw))
            );
            double ratio = totalScore / rWords.Length;

            // Ratio mis à jour pour coller au max de 25 points
            int score = ratio switch
            {
                >= 0.8 => 25, // Excellent match
                >= 0.5 => 18, // Bon match
                >= 0.3 => 12, // Match partiel
                >= 0.1 => 6,  // Faible match
                _ => 0
            };

            return (score, $"JobTitle (IA) : {pTitle} vs {rTitle} — ratio {ratio:P0} → {score}/{MaxJobTitleScore}pts");
        }

        return ScoreJobTitleMatchLegacy(profile, rfp);
    }

    public static (int score, string feedback) ScoreSkillsMatch(Profile profile, RFP rfp, NormalizedData normProfile = null, NormalizedData normRfp = null)
    {
        if (normProfile != null && normRfp != null && normRfp.NormalizedSkills.Any())
        {
            var profileSkills = normProfile.NormalizedSkills ?? new List<string>();
            int totalRfpSkills = normRfp.NormalizedSkills.Count;

            // Calcul détaillé des matches
            var matchDetails = normRfp.NormalizedSkills.Select(rs =>
            {
                double bestScore = profileSkills.Count > 0
                    ? profileSkills.Max(ps => NormalizedMatchScore(rs, ps))
                    : 0.0;
                // Seuil de validation : on considère matché si le score de similarité > 40%
                return new { Skill = rs, Score = bestScore, IsMatch = bestScore > 0.4 };
            }).ToList();

            var matchedSkills = matchDetails.Where(m => m.IsMatch).ToList();
            var missingSkills = matchDetails.Where(m => !m.IsMatch).ToList();

            int matchCount = matchedSkills.Count;

            
            // 1. Score de base : 4 points par compétence matchée
            // Ex: 5 skills matchées = 20 pts. 10 skills = 40 pts.
            int baseScore = matchCount * 4;

            // 2. Bonus de complétion (pour encourager le 100% ou la quasi-totalité)
            double ratio = (double)matchCount / totalRfpSkills;
            int bonus = 0;

            if (ratio == 1.0)
            {
                // Match parfait : on ajoute assez pour atteindre ou approcher le max selon le nombre de skills
                // Si peu de skills (ex: 3), 3*4=12, il faut un gros bonus. 
                // Si beaucoup de skills (ex: 12), 12*4=48 (déjà plafonné).
                bonus = 9; 
            }
            else if (ratio >= 0.8)
            {
                // Quasi-perfect : bonus intermédiaire
                bonus = 5;
            }
            else if (ratio >= 0.5)
            {
                // Majorité : petit bonus
                bonus = 2;
            }

            int finalScore = baseScore + bonus;

            // Plafonnement strict à 45
            if (finalScore > MaxSkillsScore)
                finalScore = MaxSkillsScore;



            string feedback = $"Skills (IA - Bonus) : {matchCount}/{totalRfpSkills} matchées → {finalScore}/{MaxSkillsScore}pts (Base: {baseScore} + Bonus: {bonus})\n" +
                              $"Matchées : [{string.Join(", ", matchedSkills.Select(m => m.Skill))}]";
            
            if (missingSkills.Any())
                feedback += $"\nManquantes : [{string.Join(", ", missingSkills.Select(m => m.Skill))}]";

            return (finalScore, feedback);
        }

        return ScoreSkillsMatchLegacy(profile, rfp);
    }

    public static (int score, string feedback) ScoreExperienceMatch(Profile profile, RFP rfp)
    {
        var inferredExperience = InferExperienceFromText(rfp.JobTitle);
        var finalExperience = inferredExperience != Experience.Unspecified
            ? inferredExperience
            : rfp.ExperienceLevel;

        if (finalExperience == Experience.Unspecified)
            return (MaxExperienceScore, "Niveau d'expérience non identifiable dans le RFP (Score max attribué par défaut).");

        if (profile.ExperienceLevel == finalExperience)
            return (MaxExperienceScore, $"Niveau d'expérience exact : {finalExperience} → {MaxExperienceScore}/{MaxExperienceScore}pts");


        return (0, $"Expérience incompatible : RFP demande {finalExperience}, profil indique {profile.ExperienceLevel} → 0/{MaxExperienceScore}pts");
    }

    public static (int score, string feedback) ScoreLocationMatch(RFP rfp)
    {
        if (string.IsNullOrWhiteSpace(rfp.Workplace))
            return (0, "Lieu de travail non précisé.");

        var tokens = TokenizeAndNormalize(rfp.Workplace);
        var brusselsVariants = new HashSet<string>
        {
            "bruxelles", "brussel", "brussels", "saintjossetennoode", "saintjosse", "regionbruxelloise"
        };

        bool isBrussels = tokens.Any(t => brusselsVariants.Contains(t));
        if (isBrussels)
            return (MaxLocationScore, $"Localisation Bruxelles détectée : '{rfp.Workplace}' → {MaxLocationScore}/{MaxLocationScore}pts");

        return (0, $"Hors Bruxelles : '{rfp.Workplace}' → 0/{MaxLocationScore}pts");
    }

    // --- LEGACY METHODS ---

    private static (int score, string feedback) ScoreJobTitleMatchLegacy(Profile profile, RFP rfp)
    {
        var rfpJobTitle = rfp.JobTitle ?? "(non renseigné)";
        var introFeedback = $"Intitulé profil : « {profile.JobTitleFr} / {profile.JobTitleEn} / {profile.JobTitleNl} »\n" +
                            $"Intitulé RFP : « {rfpJobTitle} »\n";

        if (string.IsNullOrWhiteSpace(rfp.JobTitle) ||
            (string.IsNullOrWhiteSpace(profile.JobTitleFr) &&
             string.IsNullOrWhiteSpace(profile.JobTitleEn) &&
             string.IsNullOrWhiteSpace(profile.JobTitleNl)))
            return (0, introFeedback + "Intitulé manquant.");

        var rfpWords = TokenizeAndNormalize(rfp.JobTitle);
        if (rfpWords.Count == 0)
            return (0, introFeedback + "Aucun mot significatif dans le titre RFP.");

        var allProfileWords = TokenizeAndNormalize(profile.JobTitleFr)
            .Concat(TokenizeAndNormalize(profile.JobTitleEn))
            .Concat(TokenizeAndNormalize(profile.JobTitleNl))
            .Concat(profile.Skills?.SelectMany(s =>
                TokenizeAndNormalize(s.Name).Concat(TokenizeAndNormalize(s.NameFr))
                .Concat(TokenizeAndNormalize(s.NameEn)).Concat(TokenizeAndNormalize(s.NameNl))
            ) ?? Enumerable.Empty<string>())
            .Concat(profile.Keywords?.SelectMany(k =>
                TokenizeAndNormalize(k.Name).Concat(TokenizeAndNormalize(k.NameFr))
                .Concat(TokenizeAndNormalize(k.NameEn)).Concat(TokenizeAndNormalize(k.NameNl))
            ) ?? Enumerable.Empty<string>())
            .Distinct()
            .ToList();

        var matchedPairs = rfpWords
            .SelectMany(rw => allProfileWords
                .Where(pw => WordsMatch(rw, pw))
                .Select(pw => (rw, pw)))
            .Distinct()
            .ToList();

        double ratio = (double)matchedPairs.Count / rfpWords.Count;

        int score = ratio switch
        {
            >= 0.8 => 25,
            >= 0.5 => 18,
            >= 0.3 => 12,
            >= 0.1 => 6,
            _ => 0
        };

        string feedback = matchedPairs.Any()
            ? $"{introFeedback}{matchedPairs.Count}/{rfpWords.Count} mots matchés ({(int)(ratio * 100)}%) → {score}pts"
            : $"{introFeedback}Aucun mot clé trouvé.";

        return (score, feedback);
    }

    private static (int score, string feedback) ScoreSkillsMatchLegacy(Profile profile, RFP rfp)
    {
        var rfpSkills = rfp.Skills ?? Enumerable.Empty<string>();

        var profileTokens = (profile.Skills?.SelectMany(s =>
            TokenizeAndNormalize(s.Name).Concat(TokenizeAndNormalize(s.NameFr))
            .Concat(TokenizeAndNormalize(s.NameEn)).Concat(TokenizeAndNormalize(s.NameNl))
        ) ?? Enumerable.Empty<string>())
        .Concat(profile.Keywords?.SelectMany(k =>
            TokenizeAndNormalize(k.Name).Concat(TokenizeAndNormalize(k.NameFr))
            .Concat(TokenizeAndNormalize(k.NameEn)).Concat(TokenizeAndNormalize(k.NameNl))
        ) ?? Enumerable.Empty<string>())
        .Distinct()
        .ToList();

        int matched = 0, total = 0;
        var matchedDetails = new List<string>();
        var missingDetails = new List<string>();

        foreach (var rawSkill in rfpSkills)
        {
            var (text, _) = ParseSkill(rawSkill);
            var tokens = TokenizeAndNormalize(text);
            total++;

            var hit = tokens.Select(t => profileTokens.FirstOrDefault(p => WordsMatch(p, t)))
                            .FirstOrDefault(m => m != null);

            if (hit != null)
            {
                matched++;
                matchedDetails.Add($"« {text} » ↔ « {hit} »");
            }
            else
            {
                missingDetails.Add($"« {text} »");
            }
        }

        // Ancienne logique linéaire pour le fallback
        int score = total > 0 ? (int)((double)matched / total * 45) : 0;

        var fb = new StringBuilder();
        fb.AppendLine($"Skills (Fallback) : {matched}/{total} → {score}pts");
        if (matchedDetails.Any()) fb.AppendLine("Matchées : " + string.Join(", ", matchedDetails));
        if (missingDetails.Any()) fb.AppendLine("Manquantes : " + string.Join(", ", missingDetails));

        return (score, fb.ToString().Trim());
    }

    // --- HELPERS ---

    private static readonly Regex TokenRegex = new Regex(
        @"[a-zA-Z0-9#+.]+",
        RegexOptions.Compiled | RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(250));

    private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "and", "the", "of", "in", "on", "for", "to", "with", "by", "a", "an", "le", "la", "les", "un", "une", "des", "de", "du"
    };

    private static List<string> TokenizeAndNormalize(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return new();

        var normalized = input.ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var cleaned = new string(normalized
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .ToArray());

        try
        {
            return TokenRegex.Matches(cleaned)
                .Select(m => m.Value)
                .Where(w => w.Length > 2 && !StopWords.Contains(w))
                .Distinct()
                .ToList();
        }
        catch (RegexMatchTimeoutException ex)
        {
            throw new InvalidOperationException(
                $"Échec de la tokenisation : le temps d'analyse Regex a été dépassé pour l'entrée (longueur: {input?.Length} chars).", 
                ex);
        }
    }

    private static bool WordsMatch(string w1, string w2)
    {
        if (w1 == w2) return true;
        int d = LevenshteinDistance(w1, w2);
        return d <= 1 && Math.Min(w1.Length, w2.Length) >= 4; // Légèrement assoupli à 4 chars
    }

    private static double NormalizedMatchScore(string a, string b)
    {
        var wa = a.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                  .Select(w => w.ToLowerInvariant()).ToArray();
        var wb = b.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                  .Select(w => w.ToLowerInvariant()).ToArray();

        if (WordsMatch(a.ToLowerInvariant(), b.ToLowerInvariant()))
            return 1.0;

        int matchedWords = wa.Count(w => wb.Any(w2 => WordsMatch(w, w2)));
        if (matchedWords == 0) return 0.0;

        double ratio = (double)matchedWords / Math.Max(wa.Length, wb.Length);
        return ratio * 0.6;
    }

    private static int LevenshteinDistance(string a, string b)
    {
        var dp = new int[a.Length + 1, b.Length + 1];
        for (int i = 0; i <= a.Length; i++) dp[i, 0] = i;
        for (int j = 0; j <= b.Length; j++) dp[0, j] = j;
        for (int i = 1; i <= a.Length; i++)
            for (int j = 1; j <= b.Length; j++)
                dp[i, j] = Math.Min(
                    Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1),
                    dp[i - 1, j - 1] + (a[i - 1] == b[j - 1] ? 0 : 1));
        return dp[a.Length, b.Length];
    }

    private static (string text, string importance) ParseSkill(string rawSkill)
    {
        var lower = rawSkill.ToLowerInvariant();
        string importance = lower.Contains("must have") ? "must have"
            : lower.Contains("should have") ? "should have"
            : lower.Contains("nice to have") ? "nice to have"
            : "N/A";

        string text = rawSkill.Split("(importance:", StringSplitOptions.RemoveEmptyEntries)[0].Trim();
        return (text, importance);
    }

    private static Experience InferExperienceFromText(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return Experience.Unspecified;
        var tokens = TokenizeAndNormalize(text);
        foreach (var token in tokens)
        {
            if (WordsMatch(token, "senior") || WordsMatch(token, "expert") || WordsMatch(token, "lead"))
                return Experience.Senior;
            if (WordsMatch(token, "medior") || WordsMatch(token, "confirme"))
                return Experience.Medior;
            if (WordsMatch(token, "junior"))
                return Experience.Junior;
        }
        return Experience.Unspecified;
    }
}