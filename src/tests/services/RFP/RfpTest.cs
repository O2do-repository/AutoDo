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
        var rfpService = new RfpService(context, mockMatchingService.Object);

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

        var rfpService = new RfpService(context, mockMatchingService.Object);


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
        var rfpService = new RfpService(context, mockMatchingService.Object);

        // Act
        var result = await rfpService.FilterRfpDeadlineNotReachedYet();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Test_LoadRfpFromJson_Should_Throw_FileNotFoundException_When_File_Not_Found()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var rfpService = new RfpService(context, Mock.Of<IMatchingService>());
        var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "nonExistentFile.json");

        // Mocking le fichier introuvable
        typeof(RfpService).GetField("_jsonFilePath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(rfpService, jsonFilePath);

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => rfpService.LoadRfpFromJson());
    }



    [Fact]
    public async Task Test_ImportRfpAndGenerateMatchingsAsync_Should_Rollback_On_Error()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var mockMatchingService = new Mock<IMatchingService>();
        mockMatchingService.Setup(m => m.MatchingsForRfpsAsync(It.IsAny<List<RFP>>()))
            .ThrowsAsync(new Exception("Matching failed"));

        var rfpService = new RfpService(context, mockMatchingService.Object);
        var jsonContent = @"
        [
            {
                ""Id"": ""RFP-ERROR"",
                ""SubmissionDeadline"": ""2025-05-01T00:00:00"",
                ""RawContent"": ""Error case"",
                ""Name"": ""Error Job"",
                ""Url"": ""https://error-test.com"",
                ""PlaceToWork"": ""Nowhere""
            }
        ]";
        var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "testError.json");
        File.WriteAllText(jsonFilePath, jsonContent);

        typeof(RfpService).GetField("_jsonFilePath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(rfpService, jsonFilePath);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await rfpService.ImportRfpAndGenerateMatchings());

        // Vérifier que la base ne contient aucun RFP ajouté
        var count = await context.Rfps.CountAsync();
        Assert.Equal(0, count);
    }
    [Fact]
    public void Test_LoadRfpFromJson_Should_Throw_When_Invalid_Json()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var invalidJsonContent = "{ invalid json content ";
        var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "invalid.json");
        File.WriteAllText(jsonFilePath, invalidJsonContent);

        var rfpService = new RfpService(context, Mock.Of<IMatchingService>());
        typeof(RfpService).GetField("_jsonFilePath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(rfpService, jsonFilePath);

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => rfpService.LoadRfpFromJson());
        Assert.Contains("Erreur lors de la lecture du fichier JSON", ex.Message);
    }

    [Fact]
    public async Task Test_ImportFromJsonData_Should_Update_All_Fields_Of_Existing_Rfp()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context);

        var mockMatchingService = new Mock<IMatchingService>();
        var rfpService = new RfpService(context, mockMatchingService.Object);

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
        var rfpService = new RfpService(context, mockMatchingService.Object);

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

}