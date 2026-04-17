using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class ProfileTest
{
    private AutoDoDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AutoDoDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options;
        return new AutoDoDbContext(options);
    }


    private void SeedDatabase(AutoDoDbContext context)
    {
        var javaSkill = new Skill
        {
            SkillUuid = new Guid("123e4567-e89b-12d3-a456-426614174033"),
            Name = "Java", NameFr = "Java", NameEn = "Java", NameNl = "Java"
        };
        var javascriptSkill = new Skill
        {
            SkillUuid = new Guid("123e4567-e89b-12d3-a456-426614174034"),
            Name = "JavaScript", NameFr = "JavaScript", NameEn = "JavaScript", NameNl = "JavaScript"
        };

        var architectKeyword = new Keyword
        {
            KeywordUuid = new Guid("123e4567-e89b-12d3-a456-426614174001"),
            Name = "Architect", NameFr = "Architecte", NameEn = "Architect", NameNl = "Architect"
        };
        var devOpsKeyword = new Keyword
        {
            KeywordUuid = new Guid("123e4567-e89b-12d3-a456-426614174002"),
            Name = "devOps", NameFr = "DevOps", NameEn = "DevOps", NameNl = "DevOps"
        };

        context.Skills.AddRange(javaSkill, javascriptSkill);
        context.Keywords.AddRange(architectKeyword, devOpsKeyword);

        var profile = new Profile
        {
            ProfileUuid = new Guid("123e4567-e89b-12d3-a456-426614174000"),
            Ratehour = 50,
            CV = "https://example.com/cv1.pdf",
            CVDate = new DateTime(2023, 5, 1),
            JobTitle = "Software Engineer",
            JobTitleFr = "Software Engineer",
            JobTitleEn = "Software Engineer",
            JobTitleNl = "Software Engineer",
            ExperienceLevel = Experience.Junior,
            ConsultantUuid = Guid.NewGuid(),
            Skills = new List<Skill> { javaSkill, javascriptSkill },
            Keywords = new List<Keyword> { architectKeyword, devOpsKeyword }
        };

        context.Profiles.Add(profile);
        context.SaveChanges();
    }

    // HELPER : Crée le ProfileService avec les 3 dépendances (DbContext, Translation, IA)
    private ProfileService CreateProfileService(AutoDoDbContext context, Mock<ITranslationService> mockTranslation = null)
    {
        var transService = mockTranslation ?? new Mock<ITranslationService>();
        
        // Mock de l'IA pour éviter les appels réels pendant les tests
        var mockAiService = new Mock<IAiNormalizationService>();
        mockAiService.Setup(x => x.NormalizeAsync(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>()))
            .ReturnsAsync(new NormalizedData 
            { 
                NormalizedJobTitle = "Normalized Job", 
                NormalizedSkills = new List<string> { "Skill1" }, 
                NormalizedKeywords = new List<string> { "Keyword1" } 
            });

        return new ProfileService(context, transService.Object, mockAiService.Object);
    }

    [Fact]
    public void Test_GetAllProfiles_Should_Return_Profiles()
    {
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var mockTranslationService = new Mock<ITranslationService>();
        var profileService = CreateProfileService(context, mockTranslationService);
        
        var result = profileService.GetAllProfiles();
        
        Assert.NotNull(result);
        Assert.IsType<List<Profile>>(result);
        Assert.NotEmpty(result); 
    }

    [Fact]
    public void Test_GetAllProfiles_Profile_Should_Contain_All_Attributes()
    {
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var mockTranslationService = new Mock<ITranslationService>();
        var profileService = CreateProfileService(context, mockTranslationService);
        
        var result = profileService.GetAllProfiles();
        var profile = result.FirstOrDefault();
        
        Assert.NotNull(profile);
        Assert.NotEqual(Guid.Empty, profile.ProfileUuid);
        Assert.NotEqual(0, profile.Ratehour);
        Assert.NotNull(profile.CV);
        Assert.NotEqual(default, profile.CVDate);
        Assert.NotNull(profile.JobTitle);
        Assert.NotNull(profile.JobTitleFr);
        Assert.NotNull(profile.JobTitleNl);
        Assert.NotNull(profile.JobTitleEn);
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
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var mockTranslationService = new Mock<ITranslationService>();
        var profileService = CreateProfileService(context, mockTranslationService);
        
        var result = profileService.GetAllProfiles();
        var profile = result.FirstOrDefault();
        
        Assert.NotNull(profile);
        Assert.Equal(new Guid("123e4567-e89b-12d3-a456-426614174000"), profile.ProfileUuid);
        Assert.Equal(50, profile.Ratehour);
        Assert.Equal("https://example.com/cv1.pdf", profile.CV);
        Assert.Equal(new DateTime(2023, 5, 1), profile.CVDate);
        Assert.Equal("Software Engineer", profile.JobTitle);
        Assert.Equal("Software Engineer", profile.JobTitleEn);
        Assert.Equal("Software Engineer", profile.JobTitleNl);
        Assert.Equal("Software Engineer", profile.JobTitleFr);
        Assert.Equal(Experience.Junior, profile.ExperienceLevel);
        Assert.Contains(profile.Skills, s => s.Name == "Java");
        Assert.Contains(profile.Skills, s => s.Name == "JavaScript");
        Assert.Contains(profile.Keywords, k => k.Name == "Architect");
        Assert.Contains(profile.Keywords, k => k.Name == "devOps");
    }

    [Fact]
    public async Task Test_AddProfile_Should_Throw_Exception_When_Consultant_Does_Not_Exist()
    {
        var context = GetInMemoryDbContext();
        var mockTranslationService = new Mock<ITranslationService>();
        var profileService = CreateProfileService(context, mockTranslationService);
        
        var newProfile = new Profile
        {
            ConsultantUuid = Guid.NewGuid(),
            Ratehour = 55,
            JobTitle = "Data Scientist",
            ExperienceLevel = Experience.Senior
        };

        var exception = await Assert.ThrowsAsync<Exception>(() => profileService.AddProfile(newProfile, new List<Guid>(), new List<Guid>()));
        Assert.Contains("n'existe pas", exception.Message);
    }

    [Fact]
    public async Task Test_AddProfile_Should_Successfully_Add_Profile()
    {
        var context = GetInMemoryDbContext();
        var consultantUuid = Guid.NewGuid();
        context.Consultants.Add(new Consultant
        {
            ConsultantUuid = consultantUuid,
            Name = "John Doe",
            Email = "john.doe@example.com",
            Phone = "1234567890",
            Surname = "Doe",
            Picture = "https://example.com/cv1.pdf",
            CopyCI = "https://example.com/cv1.pdf",
            enterprise = "O2do",
            Comment = ""
        });

        context.SaveChanges();  

        var mockTranslationService = new Mock<ITranslationService>();
        mockTranslationService.Setup(t => t.TranslateTextAsync(It.IsAny<string>(), It.IsAny<string>()))
                            .ReturnsAsync((string text, string lang) => $"{text}_{lang}");

        var profileService = CreateProfileService(context, mockTranslationService);
        
        var newProfile = new Profile
        {
            Ratehour = 50,
            CV = "https://example.com/cv1.pdf",
            CVDate = new DateTime(2023, 5, 1),
            JobTitle = "Software Engineer",
            ExperienceLevel = Experience.Junior,
            ConsultantUuid = consultantUuid,
            Skills = new List<Skill>
            {
                new Skill { Name = "Java", NameFr = "Java", NameEn = "Java", NameNl = "Java" },
                new Skill { Name = "JavaScript", NameFr = "JavaScript", NameEn = "JavaScript", NameNl = "JavaScript" }
            },
            Keywords = new List<Keyword>
            {
                new Keyword { Name = "Architect", NameFr = "Architecte", NameEn = "Architect", NameNl = "Architect" },
                new Keyword { Name = "devOps", NameFr = "DevOps", NameEn = "DevOps", NameNl = "DevOps" }
            }
        };

        var addedProfile = await profileService.AddProfile(newProfile, new List<Guid>(), new List<Guid>());
        var storedProfile = context.Profiles.FirstOrDefault(p => p.ProfileUuid == addedProfile.ProfileUuid);

        Assert.NotNull(storedProfile);
        Assert.Equal(consultantUuid, storedProfile.ConsultantUuid);
    }

    [Fact]
    public async Task Test_AddProfile_Should_Associate_Consultant_To_Profile()
    {
        var context = GetInMemoryDbContext();
        var consultantUuid = Guid.NewGuid();
        context.Consultants.Add(new Consultant
        {
            ConsultantUuid = consultantUuid,
            Name = "John Doe",
            Email = "john.doe@example.com",
            Phone = "1234567890",
            Surname = "Doe",
            Picture = "https://example.com/cv1.pdf",
            CopyCI = "https://example.com/cv1.pdf",
            enterprise = "O2do",
            Comment = ""                     
        });
        context.SaveChanges();  

        var mockTranslationService = new Mock<ITranslationService>();
        mockTranslationService.Setup(t => t.TranslateTextAsync(It.IsAny<string>(), It.IsAny<string>()))
                            .ReturnsAsync((string text, string lang) => $"{text}_{lang}");

        var profileService = CreateProfileService(context, mockTranslationService);

        var newProfile = new Profile
        {
            ProfileUuid = Guid.NewGuid(),
            Ratehour = 50,
            CV = "https://example.com/cv1.pdf",
            CVDate = new DateTime(2023, 5, 1),
            JobTitle = "Software Engineer",
            ExperienceLevel = Experience.Junior,
            ConsultantUuid = consultantUuid,
            Skills = new List<Skill>
            {
                new Skill { Name = "Java", NameFr = "Java", NameEn = "Java", NameNl = "Java" },
                new Skill { Name = "JavaScript", NameFr = "JavaScript", NameEn = "JavaScript", NameNl = "JavaScript" }
            },
            Keywords = new List<Keyword>
            {
                new Keyword { Name = "Architect", NameFr = "Architecte", NameEn = "Architect", NameNl = "Architect" },
                new Keyword { Name = "devOps", NameFr = "DevOps", NameEn = "DevOps", NameNl = "DevOps" }
            }
        };

        var addedProfile = await profileService.AddProfile(newProfile, new List<Guid>(), new List<Guid>());
        var storedProfile = context.Profiles.FirstOrDefault(p => p.ProfileUuid == addedProfile.ProfileUuid);

        Assert.NotNull(storedProfile);
        Assert.Equal(consultantUuid, storedProfile.ConsultantUuid);
    }

    [Fact]
    public async Task Test_UpdateProfile_Should_Throw_Exception_When_Profile_Not_Found()
    {
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var mockTranslationService = new Mock<ITranslationService>();
        var profileService = CreateProfileService(context, mockTranslationService);

        var nonExistingProfile = new Profile
        {
            ProfileUuid = Guid.NewGuid(),
            Ratehour = 60,
            CV = "https://example.com/cv2.pdf",
            CVDate = new DateTime(2024, 1, 1),
            JobTitle = "Senior Software Engineer",
            ExperienceLevel = Experience.Senior
        };

        var skillUuids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var keywordUuids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        var exception = await Assert.ThrowsAsync<Exception>(() =>
            profileService.UpdateProfile(nonExistingProfile, skillUuids, keywordUuids));

        Assert.Contains("n'existe pas", exception.Message);
    }

    [Fact]
    public void Test_DeleteProfile_Should_Remove_Profile_Successfully()
    {
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var mockTranslationService = new Mock<ITranslationService>();
        var profileService = CreateProfileService(context, mockTranslationService);

        var existingProfileUuid = new Guid("123e4567-e89b-12d3-a456-426614174000");

        var profileBeforeDelete = context.Profiles.FirstOrDefault(p => p.ProfileUuid == existingProfileUuid);
        Assert.NotNull(profileBeforeDelete);

        profileService.DeleteProfile(existingProfileUuid);

        var profileAfterDelete = context.Profiles.FirstOrDefault(p => p.ProfileUuid == existingProfileUuid);
        Assert.Null(profileAfterDelete);
    }

    [Fact]
    public void Test_DeleteProfile_Should_Throw_Exception_When_Profile_Not_Found()
    {
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var mockTranslationService = new Mock<ITranslationService>();
        var profileService = CreateProfileService(context, mockTranslationService);

        var nonExistingProfileUuid = Guid.NewGuid();

        var exception = Assert.Throws<Exception>(() => profileService.DeleteProfile(nonExistingProfileUuid));
        Assert.Contains("n'existe pas", exception.Message);
    }

    [Fact]
    public void Test_GetProfilesByConsultant_Should_Return_Profiles_When_Consultant_Has_Profiles()
    {
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var consultantUuid = context.Profiles.First().ConsultantUuid;
        var mockTranslationService = new Mock<ITranslationService>();
        var profileService = CreateProfileService(context, mockTranslationService);

        var result = profileService.GetProfilesByConsultant(consultantUuid);

        Assert.NotNull(result);  
        Assert.NotEmpty(result);
        Assert.All(result, p => Assert.Equal(consultantUuid, p.ConsultantUuid)); 
    }

    [Fact]
    public void Test_GetProfilesByConsultant_Should_Return_Empty_List_When_Consultant_Has_No_Profiles()
    {
        var context = GetInMemoryDbContext();
        var consultantUuid = Guid.NewGuid();
        var mockTranslationService = new Mock<ITranslationService>();
        var profileService = CreateProfileService(context, mockTranslationService);

        var result = profileService.GetProfilesByConsultant(consultantUuid);

        Assert.NotNull(result);  
        Assert.Empty(result);      
    }

    [Fact]
    public void Test_GetProfilesByConsultant_Should_Return_Empty_List_When_Consultant_Does_Not_Exist()
    {
        var context = GetInMemoryDbContext();
        var nonExistentConsultantUuid = Guid.NewGuid();
        var mockTranslationService = new Mock<ITranslationService>();
        var profileService = CreateProfileService(context, mockTranslationService);

        var result = profileService.GetProfilesByConsultant(nonExistentConsultantUuid);

        Assert.NotNull(result);    
        Assert.Empty(result);     
    }


}