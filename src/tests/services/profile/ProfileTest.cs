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
                Skills = new List<string> { "Java","JavaScript" },
                Keywords = new List<string> { "Architect", "devOps" }
            },
        });
        context.SaveChanges();
    }


    // list of profiles
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


    // Add profile
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
        Assert.Contains("devOps", profile.Keywords);
    }

    [Fact]
    public void Test_AddProfile_Should_Throw_Exception_When_Consultant_Does_Not_Exist()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var profileService = new ProfileService(context);
        

        var newProfile = new Profile
        {
            ConsultantUuid = Guid.NewGuid(), // UUID inexistant
            Ratehour = 55,
            JobTitle = "Data Scientist",
            ExperienceLevel = Experience.Senior
        };

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => profileService.AddProfile(newProfile));
        Assert.Contains("n'existe pas", exception.Message);
    }


    [Fact]
    public void Test_AddProfile_Should_Successfully_Add_Profile()
    {
        // Arrange
        var context = GetInMemoryDbContext();


        var consultantUuid = Guid.NewGuid();
        context.Consultants.Add(new Consultant
        {
            ConsultantUuid = consultantUuid,
            Name = "John Doe",
            Email = "john.doe@example.com", 
            Phone = "1234567890",          
            Surname = "Doe"                 
        });
        context.SaveChanges();  

        // Créer un profil et l'associer au consultant existant
        var profileService = new ProfileService(context);
        var newProfile = new Profile
        {
            Ratehour = 50,
            CV = "https://example.com/cv1.pdf",
            CVDate = new DateTime(2023, 5, 1),
            JobTitle = "Software Engineer",
            ExperienceLevel = Experience.Junior,
            ConsultantUuid = consultantUuid,  
            Skills = new List<string> { "Java", "JavaScript" },
            Keywords = new List<string> { "Architect", "devOps" }
        };

        // Act
        var addedProfile = profileService.AddProfile(newProfile);

        var storedProfile = context.Profiles.FirstOrDefault(p => p.ProfileUuid == addedProfile.ProfileUuid);

        // Assert
        Assert.NotNull(storedProfile); 
        Assert.Equal(consultantUuid, storedProfile.ConsultantUuid);  
    }

    [Fact]
    public void Test_AddProfile_Should_Associate_Consultant_To_Profile()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var consultantUuid = Guid.NewGuid();
        context.Consultants.Add(new Consultant
        {
            ConsultantUuid = consultantUuid,
            Name = "John Doe",
            Email = "john.doe@example.com", 
            Phone = "1234567890",          
            Surname = "Doe"                 
        });
        context.SaveChanges();  

        var profileService = new ProfileService(context);
        var newProfile = 
        new Profile
        {
            ProfileUuid = Guid.NewGuid(),
            Ratehour = 50,
            CV = "https://example.com/cv1.pdf",
            CVDate = new DateTime(2023, 5, 1),
            JobTitle = "Software Engineer",
            ExperienceLevel = Experience.Junior,
            ConsultantUuid = consultantUuid,
            Skills = new List<string> { "Java","JavaScript" },
            Keywords = new List<string> { "Architect", "devOps" }
        };
        // Act
        var addedProfile = profileService.AddProfile(newProfile);
        var storedProfile = context.Profiles.FirstOrDefault(p => p.ProfileUuid == addedProfile.ProfileUuid);

        // Assert
        Assert.NotNull(storedProfile);
        Assert.Equal(consultantUuid, storedProfile.ConsultantUuid);
    }



    [Fact]
    public void Test_UpdateProfile_Should_Update_Profile_Successfully()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var profileService = new ProfileService(context);

        var existingProfileUuid = new Guid("123e4567-e89b-12d3-a456-426614174000");
        var updatedProfile = new Profile
        {
            ProfileUuid = existingProfileUuid,
            Ratehour = 60,
            CV = "https://example.com/cv2.pdf",
            CVDate = new DateTime(2024, 1, 1),
            JobTitle = "Senior Software Engineer",
            ExperienceLevel = Experience.Senior,
            Skills = new List<string> { "Python", "C#" },
            Keywords = new List<string> { "AI", "Cloud" }
        };

        // Act
        profileService.UpdateProfile(updatedProfile);
        var storedProfile = context.Profiles.FirstOrDefault(p => p.ProfileUuid == existingProfileUuid);

        // Assert
        Assert.NotNull(storedProfile);
        Assert.Equal(updatedProfile.ProfileUuid, storedProfile.ProfileUuid);
        Assert.Equal(updatedProfile.Ratehour, storedProfile.Ratehour);
        Assert.Equal(updatedProfile.CV, storedProfile.CV);
        Assert.Equal(updatedProfile.CVDate, storedProfile.CVDate);
        Assert.Equal(updatedProfile.JobTitle, storedProfile.JobTitle);
        Assert.Equal(updatedProfile.ExperienceLevel, storedProfile.ExperienceLevel);
        Assert.Equal(updatedProfile.Skills, storedProfile.Skills);
        Assert.Equal(updatedProfile.Keywords, storedProfile.Keywords);
        Assert.InRange(storedProfile.Ratehour, 10, 500);
        Assert.StartsWith("https://", storedProfile.CV);
    }
    
    [Fact]
    public void Test_UpdateProfile_Should_Throw_Exception_When_Profile_Not_Found()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var profileService = new ProfileService(context);

        var nonExistingProfile = new Profile
        {
            ProfileUuid = Guid.NewGuid(),
            Ratehour = 60,
            CV = "https://example.com/cv2.pdf",
            CVDate = new DateTime(2024, 1, 1),
            JobTitle = "Senior Software Engineer",
            ExperienceLevel = Experience.Senior,
            Skills = new List<string> { "Python", "C#" },
            Keywords = new List<string> { "AI", "Cloud" }
        };

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => profileService.UpdateProfile(nonExistingProfile));
        Assert.Contains("n'existe pas", exception.Message);
    }

    [Fact]
    public void Test_DeleteProfile_Should_Remove_Profile_Successfully()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var profileService = new ProfileService(context);

        var existingProfileUuid = new Guid("123e4567-e89b-12d3-a456-426614174000");

        // Vérifier que le profil existe avant suppression
        var profileBeforeDelete = context.Profiles.FirstOrDefault(p => p.ProfileUuid == existingProfileUuid);
        Assert.NotNull(profileBeforeDelete); // Il doit exister

        // Act
        profileService.DeleteProfile(existingProfileUuid);

        // Assert
        var profileAfterDelete = context.Profiles.FirstOrDefault(p => p.ProfileUuid == existingProfileUuid);
        Assert.Null(profileAfterDelete); // Il ne doit plus exister
    }

    [Fact]
    public void Test_DeleteProfile_Should_Throw_Exception_When_Profile_Not_Found()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var profileService = new ProfileService(context);

        var nonExistingProfileUuid = Guid.NewGuid(); // UUID qui n'existe pas

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => profileService.DeleteProfile(nonExistingProfileUuid));
        Assert.Contains("n'existe pas", exception.Message);
    }



}