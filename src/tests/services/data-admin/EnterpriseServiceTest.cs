using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;


public class EnterpriseServiceTests
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
        context.Enterprises.AddRange(new List<Enterprise>
        {
            new Enterprise
            {
                EnterpriseUuid = new Guid("123e4567-e89b-12d3-a456-426614174000"),
                Name = "Enterprise A"
            },
            new Enterprise
            {
                EnterpriseUuid = new Guid("987e4567-e89b-12d3-a456-426614174111"),
                Name = "Enterprise B"
            }
        });
        context.SaveChanges();
    }

    [Fact]
    public void Test_GetAllEnterprises_Should_Return_Enterprises()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var enterpriseService = new EnterpriseService(context);

        // Act
        var result = enterpriseService.GetAllEnterprises();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<Enterprise>>(result);
        Assert.Equal(2, result.Count);  // Assuming 2 enterprises are seeded
    }

    [Fact]
    public void Test_AddEnterprise_Should_Add_Enterprise()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var enterpriseService = new EnterpriseService(context);
        var newEnterprise = new Enterprise
        {
            Name = "Enterprise C"
        };

        // Act
        var result = enterpriseService.AddEnterprise(newEnterprise);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Enterprise C", result.Name);
        Assert.NotEqual(Guid.Empty, result.EnterpriseUuid);  // Should generate a new GUID
    }

    [Fact]
    public void Test_DeleteEnterprise_Should_Remove_Enterprise_Successfully()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var enterpriseService = new EnterpriseService(context);
        var enterpriseUuid = new Guid("123e4567-e89b-12d3-a456-426614174000");

        // Act
        enterpriseService.DeleteEnterprise(enterpriseUuid);

        // Assert
        var enterprise = context.Enterprises.FirstOrDefault(e => e.EnterpriseUuid == enterpriseUuid);
        Assert.Null(enterprise); // Should be null after deletion
    }

    [Fact]
    public void Test_DeleteEnterprise_Should_Throw_Exception_When_Enterprise_Not_Found()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var enterpriseService = new EnterpriseService(context);
        var nonExistentUuid = Guid.NewGuid();  // UUID that does not exist in the database

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => enterpriseService.DeleteEnterprise(nonExistentUuid));
        Assert.Contains("L'entreprise avec UUID", exception.Message); // Check for specific error message
    }
}

