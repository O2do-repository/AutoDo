

using Microsoft.EntityFrameworkCore;

public class MatchingServiceScoringTest
{
    private AutoDoDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AutoDoDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique database per test
            .Options;
        return new AutoDoDbContext(options);
    }
    private void SeedDatabase(AutoDoDbContext context)
    {
        // Create skill and keyword objects
        var javaSkill = new Skill { SkillUuid = Guid.NewGuid(), Name = "Java" };
        var javascriptSkill = new Skill { SkillUuid = Guid.NewGuid(), Name = "JavaScript" };
        
        var architectKeyword = new Keyword { KeywordUuid = Guid.NewGuid(), Name = "Architect" };
        var devOpsKeyword = new Keyword { KeywordUuid = Guid.NewGuid(), Name = "DevOps" };
        var dataScienceKeyword = new Keyword { KeywordUuid = Guid.NewGuid(), Name = "DataScience" };

        context.Rfps.AddRange(new List<RFP>
        {
            new RFP
            {
                RFPUuid = new Guid("123e4567-e89b-12d3-a456-426614174233"),
                DeadlineDate = DateTime.Today.AddDays(5),
                DescriptionBrut = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua",
                PublicationDate = new DateTime(2023, 5, 1),
                JobTitle = "Software Engineer",
                ExperienceLevel = Experience.Junior,
                RfpUrl = "https://example.com/jobs/12345",
                Skills = new List<string> { "Java","c#" },
                Workplace = "Bruxelles",
                RfpPriority = "haute",
                Reference="RFPTest3"
            },
            new RFP
            {
                RFPUuid = new Guid("123e4567-e89b-12d3-a456-426614174234"),
                DeadlineDate = DateTime.Today.AddDays(5),
                DescriptionBrut = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua",
                PublicationDate = new DateTime(2023, 5, 1),
                JobTitle = "Data Test",
                ExperienceLevel = Experience.Junior,
                RfpUrl = "https://example.com/jobs/12345",
                Skills = new List<string>
                {
                    "Python (importance: nice to have)",
                    "Docker (importance: nice to have)"
                },
                Workplace = "Bruxelles",
                RfpPriority = "haute",
                Reference="RFPTest1"
            },
            new RFP
            {
                RFPUuid = new Guid("123e4567-e89b-12d3-a456-426614174235"),
                DeadlineDate = DateTime.Today.AddDays(5),
                DescriptionBrut = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua",
                PublicationDate = new DateTime(2023, 5, 1),
                JobTitle = "Engineer Software",
                ExperienceLevel = Experience.Junior,
                RfpUrl = "https://example.com/jobs/12345",
                Skills = new List<string>
                {
                    "Java (importance: must have)",
                    "Spring (importance: must have)"
                },
                Workplace = "Charleroi",
                RfpPriority = "haute",
                Reference="RFPTest2"
            },
        });

        context.SaveChanges();
    }


    [Fact]
    public void Test_ScoreJobTitleMatch_Should_Return_Expected_Score()
    {
        using var context = GetInMemoryDbContext();
        SeedDatabase(context);

        var profile = new Profile
        {
            ProfileUuid = Guid.NewGuid(),
            Ratehour = 50,
            CV = "https://example.com/cv1.pdf",
            CVDate = DateTime.Today,
            JobTitle = "Software Engineer",
            ExperienceLevel = Experience.Junior,
            ConsultantUuid = Guid.NewGuid(),
            Skills = new List<string> { "Java", "JavaScript" },
            Keywords = new List<string> { "Architect", "DevOps" }
        };

        var rfp = context.Rfps.First(r => r.Reference == "RFPTest3"); // Software Engineer

        var (score, _) = MatchingScoring.ScoreJobTitleMatch(profile, rfp);
        Assert.InRange(score, 10, 20);

    }

    [Fact]
    public void Test_ScoreJobTitleMatch_Should_Return_0_When_No_Match()
    {
        using var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var profile = new Profile
        {
            ProfileUuid = Guid.NewGuid(),
            Ratehour = 50,
            CV = "https://example.com/cv1.pdf",
            CVDate = DateTime.Today,
            JobTitle = "Software Engineer",
            ExperienceLevel = Experience.Junior,
            ConsultantUuid = Guid.NewGuid(),
            Skills = new List<string> { "Java", "JavaScript" },
            Keywords = new List<string> { "Architect", "DevOps" }
        };

        var rfp = context.Rfps.First(r => r.Reference == "RFPTest1"); // Data Test

        var (score, _) = MatchingScoring.ScoreJobTitleMatch(profile, rfp);
    
        Assert.Equal(0, score);
    }
    [Fact]
    public void Test_ScoreJobTitleMatch_Should_Return_15_For_Partial_Match()
    {
        var profile = new Profile
        {
            ProfileUuid = Guid.NewGuid(),
            Ratehour = 50,
            CV = "https://example.com/cv1.pdf",
            CVDate = DateTime.Today,
            JobTitle = "Software Dev",
            ExperienceLevel = Experience.Junior,
            ConsultantUuid = Guid.NewGuid(),
            Skills = new List<string> { "Java", "JavaScript" },
            Keywords = new List<string> { "Architect", "DevOps" }
        };

        var rfp = new RFP
        {
            JobTitle = "Software Engineer"
        };

        var (score, _) = MatchingScoring.ScoreJobTitleMatch(profile, rfp);

        Assert.Equal(15, score);
    }

    [Fact]
    public void Test_ScoreExperienceMatch_Should_Return_20_When_Experience_Matches()
    {
        using var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var profile = new Profile
        {
            ProfileUuid = Guid.NewGuid(),
            Ratehour = 50,
            CV = "https://example.com/cv1.pdf",
            CVDate = DateTime.Today,
            JobTitle = "Software Dev",
            ExperienceLevel = Experience.Junior,
            ConsultantUuid = Guid.NewGuid(),
            Skills = new List<string> { "Java", "JavaScript" },
            Keywords = new List<string> { "Architect", "DevOps" }
        };

        var rfp = context.Rfps.First(r => r.Reference == "RFPTest3");

        var (score, _) = MatchingScoring.ScoreExperienceMatch(profile, rfp);

        Assert.Equal(20, score);
    }
    [Fact]
    public void Test_ScoreLocationMatch_Should_Return_0_For_Unknown_Location()
    {
        using var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var rfp = context.Rfps.First(r => r.Reference == "RFPTest2"); // Software Engineer
        var (score, _) = MatchingScoring.ScoreLocationMatch(rfp);

        Assert.Equal(0, score);
    }
    [Fact]
    public void Test_ScoreLocationMatch_Should_Handle_Brussels_Variants()
    {
        using var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var rfp = context.Rfps.First(r => r.Reference == "RFPTest1"); // Software Engineer

        var (score, _) = MatchingScoring.ScoreLocationMatch(rfp);

        Assert.Equal(20, score);
    }

    [Fact]
    public void Test_ScoreSkillsMatch_Should_Partially_Score_For_Required_Skills()
    {
        using var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var rfp = context.Rfps.First(r => r.Reference == "RFPTest2"); 
        var profile = new Profile
        {
            ProfileUuid = Guid.NewGuid(),
            Ratehour = 50,
            CV = "https://example.com/cv1.pdf",
            CVDate = DateTime.Today,
            JobTitle = "Software Dev",
            ExperienceLevel = Experience.Junior,
            ConsultantUuid = Guid.NewGuid(),
            Skills = new List<string> { "Java", "JavaScript" },
            Keywords = new List<string> { "Architect", "DevOps" }
        };

        var (score, _) = MatchingScoring.ScoreSkillsMatch(profile, rfp);

        Assert.True(score > 0 && score < 35);
    }
    [Fact]
    public void Test_ScoreSkillsMatch_Should_Score_Only_NiceToHave()
    {
        using var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var rfp = context.Rfps.First(r => r.Reference == "RFPTest1"); 
        var profile = new Profile
        {
            ProfileUuid = Guid.NewGuid(),
            Ratehour = 50,
            CV = "https://example.com/cv1.pdf",
            CVDate = DateTime.Today,
            JobTitle = "Software Dev",
            ExperienceLevel = Experience.Junior,
            ConsultantUuid = Guid.NewGuid(),
            Skills = new List<string> { "Python", "Docker" },
            Keywords = new List<string> { "Architect", "DevOps" }
        };

        var (score, _) = MatchingScoring.ScoreSkillsMatch(profile, rfp);

        Assert.True(score == 5);
    }



}