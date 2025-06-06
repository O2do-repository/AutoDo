using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique database per test
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
            }, // Expired

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
            }, // Valid

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
            }, // Valid

        });
        context.SaveChanges();
    }

    [Fact]
    public async Task Test_FilterRfpDeadlineNotReachedYet_Should_Not_Return_Expired_Rfps()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context);

        // Mocking IMatchingService
        var mockMatchingService = new Mock<IMatchingService>();

        // Instancier le service avec le mock de IMatchingService
        var mockTranslationService = new Mock<ITranslationService>();
        var rfpService = new RfpService(context, mockMatchingService.Object, mockTranslationService.Object);


        // Act
        var result = await rfpService.FilterRfpDeadlineNotReachedYet();

        // Assert
        Assert.DoesNotContain(result, rfp => rfp.DeadlineDate < DateTime.Today);
    }

    [Fact]
    public async Task Test_FilterRfpDeadlineNotReachedYet_Should_Return_Today_And_Future_Rfps()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var mockMatchingService = new Mock<IMatchingService>();

        var mockTranslationService = new Mock<ITranslationService>();
        var rfpService = new RfpService(context, mockMatchingService.Object, mockTranslationService.Object);



        // Act
        var result = await rfpService.FilterRfpDeadlineNotReachedYet();

        // Assert
        Assert.Contains(result, rfp => rfp.DeadlineDate == DateTime.Today);
        Assert.Contains(result, rfp => rfp.DeadlineDate > DateTime.Today);
    }

    [Fact]
    public async Task Test_FilterRfpDeadlineNotReachedYet_Should_Return_Empty_List_When_No_Valid_Rfps()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.Rfps.Add(new RFP
        {
            RFPUuid = Guid.NewGuid(),
            DeadlineDate = DateTime.Today.AddDays(-5),
            Skills = new List<string> { "C#", "Vue.js" },
            DescriptionBrut = "Junior Web Developer for eCommerce site",
            JobTitle = "Junior Developer",
            Reference = "RFP-11223",
            RfpPriority = "Low",
            RfpUrl = "https://example.com/rfp/11223",
            Workplace = "On-site"
        });
        context.SaveChanges();
        var mockMatchingService = new Mock<IMatchingService>();

        // Instancier le service avec le mock de IMatchingService
        var mockTranslationService = new Mock<ITranslationService>();
        var rfpService = new RfpService(context, mockMatchingService.Object, mockTranslationService.Object);


        // Act
        var result = await rfpService.FilterRfpDeadlineNotReachedYet();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Test_ImportFromJsonData_Should_Update_All_Fields_Of_Existing_Rfp()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context);

        var mockMatchingService = new Mock<IMatchingService>();
        var mockTranslationService = new Mock<ITranslationService>();
        var rfpService = new RfpService(context, mockMatchingService.Object, mockTranslationService.Object);


        var updatedRfp = new RFP
        {
            Reference = "RFP-12345", // déjà présent dans la seed
            JobTitle = "Updated Title",
            DescriptionBrut = "Updated Description",
            PublicationDate = new DateTime(2025, 4, 22),
            DeadlineDate = DateTime.Today.AddDays(30),
            RfpUrl = "https://updated-url.com",
            Workplace = "Updated Workplace",
            RfpPriority = "Urgent",
            Skills = new List<string> { "C#", "Vue.js" }
        };

        // Act
        await rfpService.ImportFromJsonData(new List<RFP> { updatedRfp });

        // Assert
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
    public async Task Test_ImportFromJsonData_Should_Add_New_Rfps_And_Trigger_Matching_With_Only_Them()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context);

        var mockMatchingService = new Mock<IMatchingService>();
        var mockTranslationService = new Mock<ITranslationService>();
        var rfpService = new RfpService(context, mockMatchingService.Object, mockTranslationService.Object);


        var newRfps = new List<RFP>
        {
            new RFP
            {
                Reference = "RFP-NEW-001",
                JobTitle = "Data Engineer",
                DescriptionBrut = "AWS and Python required",
                PublicationDate = DateTime.Today,
                DeadlineDate = DateTime.Today.AddDays(5),
                RfpUrl = "https://example.com/rfp/new001",
                Workplace = "Remote",
                RfpPriority = "High",
                Skills = new List<string> { "Python", "AWS" }
            },
            new RFP
            {
                Reference = "RFP-NEW-002",
                JobTitle = "DevOps Specialist",
                DescriptionBrut = "Kubernetes + Terraform",
                PublicationDate = DateTime.Today,
                DeadlineDate = DateTime.Today.AddDays(10),
                RfpUrl = "https://example.com/rfp/new002",
                Workplace = "On-site",
                RfpPriority = "Medium",
                Skills = new List<string> { "Kubernetes", "Terraform" }
            }
        };

        // Act
        await rfpService.ImportFromJsonData(newRfps);

        // Assert
        var rfpsInDb = context.Rfps.Where(r => r.Reference.StartsWith("RFP-NEW")).ToList();
        Assert.Equal(2, rfpsInDb.Count);

        mockMatchingService.Verify(m => m.MatchingsForRfpsAsync(
            It.Is<List<RFP>>(list => list.All(r => r.Reference.StartsWith("RFP-NEW")) && list.Count == 2)), Times.Once);
    }
    [Fact]
    public void Test_DeleteOldRFPs_Should_Delete_RFPs_When_Deadline_Is_Expired()
    {
        // Arrange
        var context = GetInMemoryDbContext();

        // Seed the database with one expired and one active RFP
        var expiredRfp = new RFP
        {
            RFPUuid = Guid.NewGuid(),
            Reference = "RFP-001",
            DeadlineDate = DateTime.Today.AddDays(-5), // Expired
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
            DeadlineDate = DateTime.Today.AddDays(5), // Still valid
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
        var mockTranslationService = new Mock<ITranslationService>();
        var rfpService = new RfpService(context, mockMatchingService.Object, mockTranslationService.Object);

        // Act
        rfpService.DeleteOldRFPs();

        // Assert
        var remainingRfps = context.Rfps.ToList();
        Assert.Single(remainingRfps);
        Assert.Equal("RFP-002", remainingRfps.First().Reference);
    }


}