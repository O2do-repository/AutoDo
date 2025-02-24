public static class DummyProfilData
{
    public static List<Profil> Profils = new List<Profil>
    {
        new Profil
        {
            Uuid = "123e4567-e89b-12d3-a456-426614174000",
            Ratehour = 50,
            CV = "https://example.com/cv1.pdf",
            CV_Date = new DateTime(2023, 5, 1),
            Job_title = "Software Engineer",
            Experience_level = Experience.Junior,
            Skills = new List<string> { Skill.Java, Skill.JavaScript },
            Keywords = new List<string> { Keyword.Architect, Keyword.DevOps }
        },
        new Profil
        {
            Uuid = "456e7890-e12b-34d5-a678-426614174001",
            Ratehour = 70,
            CV = "https://example.com/cv2.pdf",
            CV_Date = new DateTime(2024, 2, 15),
            Job_title = "Cyber Security Analyst",
            Experience_level = Experience.Medior,
            Skills = new List<string> { Skill.Csharp, Skill.Python },
            Keywords = new List<string> { Keyword.CyberSecurity, Keyword.DataScience }
        },
        new Profil
        {
            Uuid = "789e0123-e34b-56d7-a890-426614174002",
            Ratehour = 90,
            CV = "https://example.com/cv3.pdf",
            CV_Date = new DateTime(2022, 11, 10),
            Job_title = "Senior Software Architect",
            Experience_level = Experience.Senior,
            Skills = new List<string> { Skill.Java, Skill.Csharp, Skill.Python },
            Keywords = new List<string> { Keyword.Architect, Keyword.DevOps, Keyword.CyberSecurity }
        }
    };
}




