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
    public void Test_LoadRfpFromJson_Should_Add_New_Rfps()
    {
        var context = GetInMemoryDbContext();
        var jsonContent =  @"
        [
            {
                ""Id"": ""VRT345923"",
                ""SubmissionDeadline"": ""2025-04-02T00:00:00"",
                ""Skills"": [""Aantoonbare ervaring als Team Coach""],
                ""RawContent"": ""Teamcoach Radio en Audio (freelance)"",
                ""Name"": ""Teamcoach Radio en Audio team (freelance)"",
                ""RfpPriority"": ""not provided"",
                ""Description"": ""blabla"",
                ""Url"": ""https://customer.connecting-expertise.com/index.php?event=seller.selectRfq&id=174308220566007346"",
                ""PlaceToWork"": ""OMROEPCENTRUM VRT (AUGUSTE REYERSLAAN, 1043 VRT, BELGIQUE)""
            }
        ]"; 


        var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "testRfp.json");
        File.WriteAllText(jsonFilePath, jsonContent);

        var rfpService = new RfpService(context, Mock.Of<IMatchingService>());
        typeof(RfpService).GetField("_jsonFilePath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(rfpService, jsonFilePath);

        rfpService.LoadRfpFromJson();

        var rfp = context.Rfps.SingleOrDefault(r => r.Reference == "VRT345923");
        Assert.NotNull(rfp);
    }
    [Fact]
    public void Test_LoadRfpFromJson_Should_Update_Existing_Rfp()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var existingRfp = new RFP
        {
            RFPUuid = Guid.NewGuid(),
            Reference = "VRT345923",
            JobTitle = "Old Title",
            DeadlineDate = DateTime.Today.AddDays(10),
            RfpUrl = "https://old-url.com",
            Workplace = "Old Location",
            Skills = new List<string> { "Java","JavaScript" },
            DescriptionBrut="aaa",
            RfpPriority="High"
        };
        context.Rfps.Add(existingRfp);
        context.SaveChanges();

        var jsonContent = @"
        [
            {
                ""Id"": ""VRT345923"",
                ""SubmissionDeadline"": ""2025-04-10T00:00:00"",
                ""RawContent"": ""Updated content"",
                ""Name"": ""Updated Job Title"",
                ""Url"": ""https://new-url.com"",
                ""PlaceToWork"": ""New Location"",
                ""Skills"": [] 
            }
        ]";
        
        var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "testUpdateRfp.json");
        File.WriteAllText(jsonFilePath, jsonContent);

        var rfpService = new RfpService(context, Mock.Of<IMatchingService>());
        typeof(RfpService).GetField("_jsonFilePath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(rfpService, jsonFilePath);

        // Act
        rfpService.LoadRfpFromJson();

        // Assert
        var updatedRfp = context.Rfps.SingleOrDefault(r => r.Reference == "VRT345923");
        Assert.NotNull(updatedRfp);
        Assert.Equal("Updated Job Title", updatedRfp.JobTitle);
        Assert.Equal("https://new-url.com", updatedRfp.RfpUrl);
        Assert.Equal("New Location", updatedRfp.Workplace);
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
}