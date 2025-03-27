using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class MatchingServiceTest
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
                Workplace = "Vezon",
                RfpPriority = "haute",
                Reference="RFPTest3"
            },
            new RFP
            {
                RFPUuid = new Guid("123e4567-e89b-12d3-a456-426614174234"),
                DeadlineDate = DateTime.Today.AddDays(5),
                DescriptionBrut = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua",
                PublicationDate = new DateTime(2023, 5, 1),
                JobTitle = "Engineer Data",
                ExperienceLevel = Experience.Junior,
                RfpUrl = "https://example.com/jobs/12345",
                Skills = new List<string> { "Python" },
                Workplace = "Vezon",
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
                Skills = new List<string> { "Java" },
                Workplace = "Vezon",
                RfpPriority = "haute",
                Reference="RFPTest2"
            },
        });

        context.SaveChanges();
    }

    [Fact]
    public async Task Test_MatchingsForProfileAsync_Should_Create_New_Matchings()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        SeedDatabase(dbContext);

        var service = new MatchingService(dbContext);

        var profile =  new Profile
        {
            ProfileUuid = new Guid("123e4567-e89b-12d3-a456-426614174000"),
            Ratehour = 50,
            CV = "https://example.com/cv1.pdf",
            CVDate = new DateTime(2023, 5, 1),
            JobTitle = "Software Engineer",
            ExperienceLevel = Experience.Junior,
            ConsultantUuid = Guid.NewGuid(),
            Skills = new List<string> { "Java","JavaScript" },
            Keywords = new List<string> { "Architect", "devOps" }
        };

        dbContext.Profiles.Add(profile);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await service.MatchingsForProfileAsync(profile);

        // Assert
        Assert.NotEmpty(result); // Vérifie que des matchings existent
        Assert.All(result, m => Assert.True(m.Score >= 0));
    }

    [Fact]
    public async Task Test_MatchingsForProfileAsync_Should_Erase_Existent_Matchings()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        SeedDatabase(dbContext);

        var service = new MatchingService(dbContext);

        var profile =  new Profile
        {
            ProfileUuid = new Guid("123e4567-e89b-12d3-a456-426614174000"),
            Ratehour = 50,
            CV = "https://example.com/cv1.pdf",
            CVDate = new DateTime(2023, 5, 1),
            JobTitle = "Software Engineer",
            ExperienceLevel = Experience.Junior,
            ConsultantUuid = Guid.NewGuid(),
            Skills = new List<string> { "Java","JavaScript" },
            Keywords = new List<string> { "Architect", "devOps" }
        };
        var matching = new Matching{
            MatchingUuid= new Guid(),
            ProfileUuid= new Guid("123e4567-e89b-12d3-a456-426614174000"),
            RfpUuid=new Guid("123e4567-e89b-12d3-a456-426614174235"),
            Score = 100,
            Comment = "test",
            StatutMatching = StatutMatching.New
        };

        dbContext.Matchings.Add(matching);
        dbContext.Profiles.Add(profile);
        await dbContext.SaveChangesAsync();

        // Act  
        var result = await service.MatchingsForProfileAsync(profile);
        // Vérifier que les anciens matchings ont bien été supprimés
        var matchingsAfter = dbContext.Matchings.Where(m => m.ProfileUuid == profile.ProfileUuid).ToList();
        // Assert
        Assert.DoesNotContain(matchingsAfter, m => m.Comment == "test");
    }

    [Fact]
    public async Task Test_MatchingsForProfileAsync_Should_Associate_Matchings_With_Correct_Profile()
    {
        var dbContext = GetInMemoryDbContext();
        SeedDatabase(dbContext);

        var service = new MatchingService(dbContext);

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

        dbContext.Profiles.Add(profile);
        await dbContext.SaveChangesAsync();

        var result = await service.MatchingsForProfileAsync(profile);

        Assert.All(result, m => Assert.Equal(profile.ProfileUuid, m.ProfileUuid));
    }
    [Fact]
    public async Task Test_GetAllMatchingsAsync_Should_Include_Rfp_And_Profile()
    {
        var dbContext = GetInMemoryDbContext();
        SeedDatabase(dbContext);

        var service = new MatchingService(dbContext);

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

        dbContext.Profiles.Add(profile);
        await dbContext.SaveChangesAsync();

        await service.MatchingsForProfileAsync(profile);

        // Act
        var result = await service.GetAllMatchingsAsync();

        // Assert : Vérifie que chaque matching contient un profil et un RFP
        Assert.All(result, m =>
        {
            Assert.NotNull(m.Profile);
            Assert.NotNull(m.Rfp);
        });
    }

    [Fact]
public async Task Test_UpdateMatchingAsync_Should_Update_Existing_Matching()
{
    // Arrange
    var dbContext = GetInMemoryDbContext();
    var service = new MatchingService(dbContext);

    var matchingId = Guid.NewGuid();
    var existingMatching = new Matching
    {
        MatchingUuid = matchingId,
        StatutMatching = StatutMatching.New,
        Comment = "Old Comment",
        Score = 50,
        ProfileUuid = Guid.NewGuid(),
        RfpUuid = Guid.NewGuid()
    };

    dbContext.Matchings.Add(existingMatching);
    await dbContext.SaveChangesAsync();

    var updatedMatching = new Matching
    {
        MatchingUuid = matchingId,
        StatutMatching = StatutMatching.Apply,
        Comment = "Updated Comment",
        Score = 90,
        ProfileUuid = existingMatching.ProfileUuid,
        RfpUuid = existingMatching.RfpUuid
    };

    // Act
    var result = await service.UpdateMatchingAsync(matchingId, updatedMatching);

    // Assert
    Assert.Equal(updatedMatching.StatutMatching, result.StatutMatching);
    Assert.Equal(updatedMatching.Comment, result.Comment);
    Assert.Equal(updatedMatching.Score, result.Score);
}

[Fact]
public async Task Test_UpdateMatchingAsync_Should_Throw_Exception_When_Matching_Not_Found()
{
    // Arrange
    var dbContext = GetInMemoryDbContext();
    var service = new MatchingService(dbContext);
    var nonExistentId = Guid.NewGuid();
    var updatedMatching = new Matching
    {
        MatchingUuid = nonExistentId,
        StatutMatching = StatutMatching.Apply,
        Comment = "Updated Comment",
        Score = 90,
        ProfileUuid = Guid.NewGuid(),
        RfpUuid = Guid.NewGuid()
    };

    // Act & Assert
    var exception = await Assert.ThrowsAsync<Exception>(async () =>
        await service.UpdateMatchingAsync(nonExistentId, updatedMatching));
    
    Assert.Contains("n'existe pas", exception.Message);
}
}
