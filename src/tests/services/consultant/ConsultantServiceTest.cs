using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

public class ConsultantServiceTest
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
        context.Consultants.AddRange(new List<Consultant>
        {
            new Consultant
            {
                ConsultantUuid = Guid.NewGuid(),
                Name = "Alice",
                Surname = "Johnson",
                Email = "alice.johnson@example.com",
                Phone = "1234567890",
                AvailabilityDate = new DateTime(2024, 5, 1),
                ExpirationDateCI = new DateTime(2025, 5, 1),
                Intern = false,
                enterprise = Enterprise.O2do,
                Profiles = new List<Profile>()
            },
            new Consultant
            {
                ConsultantUuid = Guid.NewGuid(),
                Name = "Bob",
                Surname = "Smith",
                Email = "bob.smith@example.com",
                Phone = "0987654321",
                AvailabilityDate = new DateTime(2024, 6, 1),
                ExpirationDateCI = new DateTime(2025, 6, 1),
                Intern = true,
                enterprise = Enterprise.MI6,
                Profiles = new List<Profile>()
            }
        });
        context.SaveChanges();
    }


    [Fact]
    public void Test_GetAllConsultants_Should_Return_Consultants()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var consultantService = new ConsultantService(context);

        // Act
        var result = consultantService.GetAllConsultants();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<Consultant>>(result);
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count);

        var firstConsultant = result.First();
        Assert.NotEqual(Guid.Empty, firstConsultant.ConsultantUuid);
        Assert.False(string.IsNullOrEmpty(firstConsultant.Name));
        Assert.False(string.IsNullOrEmpty(firstConsultant.Surname));
        Assert.False(string.IsNullOrEmpty(firstConsultant.Email));
        Assert.False(string.IsNullOrEmpty(firstConsultant.Phone));
        Assert.NotEqual(default(DateTime), firstConsultant.AvailabilityDate);
        Assert.NotEqual(default(DateTime), firstConsultant.ExpirationDateCI);
        Assert.NotNull(firstConsultant.enterprise);
        Assert.NotNull(firstConsultant.Profiles);
    }

    [Fact]
    public void Test_GetAllConsultants_Should_Return_Empty_List_When_No_Consultants()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var consultantService = new ConsultantService(context);

        // Act
        var result = consultantService.GetAllConsultants();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<Consultant>>(result);
        Assert.Empty(result);
    }
}
