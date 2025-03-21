using Microsoft.EntityFrameworkCore;
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
        var rfpService = new RfpService(context);

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
        var rfpService = new RfpService(context);

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
        var rfpService = new RfpService(context);

        // Act
        var result = await rfpService.FilterRfpDeadlineNotReachedYet();

        // Assert
        Assert.Empty(result);
    }
}