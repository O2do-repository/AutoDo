using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class RfpTest
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
        context.Rfps.AddRange(new List<RFP>
        {
            new RFP
            {
                RFPUuid = Guid.NewGuid(),
                DeadlineDate = DateTime.Today.AddDays(-1),
                Skills = new List<string> { "C#", "Vue.js" },
                DescriptionBrut = "Senior C# Developer for web application",
                JobTitle = "Senior Developer",
                Reference = "RFP-12345",
                RfpPriority = "High",
                RfpUrl = "https://example.com/rfp/12345",
                Workplace = "Remote"
            },
            new RFP
            {
                RFPUuid = Guid.NewGuid(),
                DeadlineDate = DateTime.Today,
                Skills = new List<string> { "C#", "Vue.js" },
                DescriptionBrut = "Full Stack Developer for SaaS platform",
                JobTitle = "Full Stack Developer",
                Reference = "RFP-67890",
                RfpPriority = "Medium",
                RfpUrl = "https://example.com/rfp/67890",
                Workplace = "Hybrid"
            },
            new RFP
            {
                RFPUuid = Guid.NewGuid(),
                DeadlineDate = DateTime.Today.AddDays(5),
                Skills = new List<string> { "C#", "Vue.js" },
                DescriptionBrut = "Junior Web Developer for eCommerce site",
                JobTitle = "Junior Developer",
                Reference = "RFP-11223",
                RfpPriority = "Low",
                RfpUrl = "https://example.com/rfp/11223",
                Workplace = "On-site"
            }
        });
        context.SaveChanges();
    }

    // HELPER : Crée le RfpService avec les 3 dépendances (DbContext, Matching, IA)
    private RfpService CreateRfpService(AutoDoDbContext context, Mock<IMatchingService> mockMatching = null)
    {
        var matchingService = mockMatching ?? new Mock<IMatchingService>();
        
        // Mock de l'IA
        var mockAiService = new Mock<IAiNormalizationService>();
        mockAiService.Setup(x => x.NormalizeAsync(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<List<string>>()))
            .ReturnsAsync(new NormalizedData 
            { 
                NormalizedJobTitle = "Normalized Job", 
                NormalizedSkills = new List<string> { "Skill1" }, 
                NormalizedKeywords = new List<string>() 
            });

        return new RfpService(context, matchingService.Object, mockAiService.Object);
    }

    [Fact]
    public async Task Test_FilterRfpDeadlineNotReachedYet_Should_Not_Return_Expired_Rfps()
    {
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var mockMatchingService = new Mock<IMatchingService>();
        var rfpService = CreateRfpService(context, mockMatchingService);

        var result = await rfpService.FilterRfpDeadlineNotReachedYet();

        Assert.DoesNotContain(result, rfp => rfp.DeadlineDate < DateTime.Today);
    }

    [Fact]
    public async Task Test_FilterRfpDeadlineNotReachedYet_Should_Return_Today_And_Future_Rfps()
    {
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var mockMatchingService = new Mock<IMatchingService>();
        var rfpService = CreateRfpService(context, mockMatchingService);

        var result = await rfpService.FilterRfpDeadlineNotReachedYet();

        Assert.Contains(result, rfp => rfp.DeadlineDate == DateTime.Today);
        Assert.Contains(result, rfp => rfp.DeadlineDate > DateTime.Today);
    }

    [Fact]
    public async Task Test_FilterRfpDeadlineNotReachedYet_Should_Return_Empty_List_When_No_Valid_Rfps()
    {
        var context = GetInMemoryDbContext();
        context.Rfps.Add(new RFP
        {
            RFPUuid = Guid.NewGuid(),
            DeadlineDate = DateTime.Today.AddDays(-5),
            Skills = new List<string> { "C#", "Vue.js" },
            DescriptionBrut = "Junior Web Developer",
            JobTitle = "Junior Developer",
            Reference = "RFP-11223",
            RfpPriority = "Low",
            RfpUrl = "https://example.com/rfp/11223",
            Workplace = "On-site"
        });
        context.SaveChanges();
        
        var mockMatchingService = new Mock<IMatchingService>();
        var rfpService = CreateRfpService(context, mockMatchingService);

        var result = await rfpService.FilterRfpDeadlineNotReachedYet();

        Assert.Empty(result);
    }

    [Fact]
    public async Task Test_ImportFromJsonData_Should_Update_All_Fields_Of_Existing_Rfp()
    {
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var mockMatchingService = new Mock<IMatchingService>();
        var rfpService = CreateRfpService(context, mockMatchingService);

        var updatedRfp = new RFP
        {
            Reference = "RFP-12345",
            JobTitle = "Updated Title",
            DescriptionBrut = "Updated Description",
            PublicationDate = new DateTime(2025, 4, 22),
            DeadlineDate = DateTime.Today.AddDays(30),
            RfpUrl = "https://updated-url.com",
            Workplace = "Updated Workplace",
            RfpPriority = "Urgent",
            Skills = new List<string> { "C#", "Vue.js" }
        };

        await rfpService.ImportFromJsonData(new List<RFP> { updatedRfp });

        var rfpInDb = context.Rfps.Single(r => r.Reference == "RFP-12345");
        Assert.Equal("Updated Title", rfpInDb.JobTitle);
        Assert.Equal("Updated Description", rfpInDb.DescriptionBrut);
        Assert.Equal(new DateTime(2025, 4, 22), rfpInDb.PublicationDate);
        Assert.Equal(DateTime.Today.AddDays(30), rfpInDb.DeadlineDate);
        Assert.Equal("https://updated-url.com", rfpInDb.RfpUrl);
        Assert.Equal("Updated Workplace", rfpInDb.Workplace);
        Assert.Equal("Urgent", rfpInDb.RfpPriority);
        Assert.Equal(new List<string> { "C#", "Vue.js" }, rfpInDb.Skills);
    }

 

    [Fact]
    public void Test_DeleteOldRFPs_Should_Delete_RFPs_When_Deadline_Is_Expired()
    {
        var context = GetInMemoryDbContext();

        var expiredRfp = new RFP
        {
            RFPUuid = Guid.NewGuid(),
            Reference = "RFP-001",
            DeadlineDate = DateTime.Today.AddDays(-5),
            DescriptionBrut = "Description expirée",
            ExperienceLevel = Experience.Junior,
            Skills = new List<string> { "C#", "ASP.NET Core" },
            JobTitle = "Développeur .NET",
            RfpUrl = "http://example.com/rfp-001",
            Workplace = "Paris",
            PublicationDate = DateTime.UtcNow.AddDays(-10),
            RfpPriority = "High"
        };

        var activeRfp = new RFP
        {
            RFPUuid = Guid.NewGuid(),
            Reference = "RFP-002",
            DeadlineDate = DateTime.Today.AddDays(5),
            DescriptionBrut = "Nouvelle mission",
            ExperienceLevel = Experience.Senior,
            Skills = new List<string> { "React", "Node.js" },
            JobTitle = "Développeur Fullstack",
            RfpUrl = "http://example.com/rfp-002",
            Workplace = "Lyon",
            PublicationDate = DateTime.UtcNow.AddDays(-2),
            RfpPriority = "Medium"
        };

        context.Rfps.AddRange(expiredRfp, activeRfp);
        context.SaveChanges();

        var mockMatchingService = new Mock<IMatchingService>();
        var rfpService = CreateRfpService(context, mockMatchingService);

        rfpService.DeleteOldRFPs();

        var remainingRfps = context.Rfps.ToList();
        Assert.Single(remainingRfps);
        Assert.Equal("RFP-002", remainingRfps.First().Reference);
    }
}