using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

public class ProfileTest
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

        context.Profiles.AddRange(new List<Profile>
        {
            new Profile
            {
                ProfileUuid = new Guid("123e4567-e89b-12d3-a456-426614174000"),
                Ratehour = 50,
                CV = "https://example.com/cv1.pdf",
                CVDate = new DateTime(2023, 5, 1),
                JobTitle = "Software Engineer",
                ExperienceLevel = Experience.Junior,
                ConsultantUuid = Guid.NewGuid(),
                Skills = new List<string> { "javaS", "javascriptS" },
                Keywords = new List<string> { "architect", "devOps" }
            },
        });
        context.SaveChanges();
    }

    [Fact]
    public void Test_GetAllProfiles_Should_Return_Profiles()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var profileService = new ProfileService(context);
        
        // Act
        var result = profileService.GetAllProfiles();
        
        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<Profile>>(result);
        Assert.NotEmpty(result); 
    }

    [Fact]
    public void Test_GetAllProfiles_Profile_Should_Contain_All_Attributes()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var profileService = new ProfileService(context);
        
        // Act
        var result = profileService.GetAllProfiles();
        var profile = result.FirstOrDefault();
        
        // Assert
        Assert.NotNull(profile);
        Assert.NotEqual(Guid.Empty, profile.ProfileUuid);
        Assert.NotEqual(0, profile.Ratehour);
        Assert.NotNull(profile.CV);
        Assert.NotEqual(default, profile.CVDate);
        Assert.NotNull(profile.JobTitle);
        Assert.True(Enum.IsDefined(typeof(Experience), profile.ExperienceLevel));
        Assert.NotNull(profile.Skills);
        Assert.NotEmpty(profile.Skills);
        Assert.NotNull(profile.Keywords);
        Assert.NotEmpty(profile.Keywords);
        Assert.NotEqual(Guid.Empty, profile.ConsultantUuid);
    }

    [Fact]
    public void Test_GetAllProfiles_Profile_Should_be_equals_to_storeProfile()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var profileService = new ProfileService(context);
        
        // Act
        var result = profileService.GetAllProfiles();
        var profile = result.FirstOrDefault();
        
        // Assert
        Assert.NotNull(profile);
        Assert.Equal(new Guid("123e4567-e89b-12d3-a456-426614174000"), profile.ProfileUuid);
        Assert.Equal(50, profile.Ratehour);
        Assert.Equal("https://example.com/cv1.pdf", profile.CV);
        Assert.Equal(new DateTime(2023, 5, 1), profile.CVDate);
        Assert.Equal("Software Engineer", profile.JobTitle);
        Assert.Equal(Experience.Junior, profile.ExperienceLevel);
        Assert.Contains("Java", profile.Skills);
        Assert.Contains("JavaScript", profile.Skills);
        Assert.Contains("Architect", profile.Keywords);
        Assert.Contains("DevOps", profile.Keywords);
    }
}