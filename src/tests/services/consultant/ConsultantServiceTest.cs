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
                Picture = "http://localhost:8888/",
                CopyCI = "http://localhost:8888/",
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
                Picture = "http://localhost:8888/",
                CopyCI = "http://localhost:8888/",
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
    [Fact]
    public void Test_AddConsultant_Should_Add_Consultant()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var consultantService = new ConsultantService(context);
        var newConsultant = new Consultant
        {
            ConsultantUuid = Guid.NewGuid(),
            Name = "John",
            Surname = "Doe",
            Email = "john.doe@example.com",
            Phone = "123456789",
            AvailabilityDate = new DateTime(2024, 7, 1),
            ExpirationDateCI = new DateTime(2025, 7, 1),
            Intern = false,
            Picture = "http://localhost:8888/",
            CopyCI = "http://localhost:8888/",
            enterprise = Enterprise.O2do,
            Profiles = new List<Profile>()
        };

        // Act
        var result = consultantService.AddConsultant(newConsultant);
        
        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.ConsultantUuid);
        Assert.Equal("John", result.Name);
        Assert.Equal("Doe", result.Surname);
        Assert.Equal("john.doe@example.com", result.Email);
        Assert.Equal("123456789", result.Phone);
        Assert.Equal("http://localhost:8888/", result.CopyCI);
        Assert.Equal("http://localhost:8888/", result.Picture);
        Assert.NotEqual(default(DateTime), result.ExpirationDateCI);
        
        var dbConsultant = context.Consultants.Find(result.ConsultantUuid);
        Assert.NotNull(dbConsultant);
    }

    [Fact]
    public void Test_GetConsultantById_Should_Return_Consultant()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var consultantService = new ConsultantService(context);
        var existingConsultant = context.Consultants.First();

        // Act
        var result = consultantService.GetConsultantById(existingConsultant.ConsultantUuid);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existingConsultant.ConsultantUuid, result.ConsultantUuid);
        Assert.Equal(existingConsultant.Name, result.Name);
        Assert.Equal(existingConsultant.Email, result.Email);
    }

    [Fact]
    public void Test_GetConsultantById_Should_Throw_Exception_When_Not_Found()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var consultantService = new ConsultantService(context);
        var nonExistentUuid = Guid.NewGuid();

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => consultantService.GetConsultantById(nonExistentUuid));
        Assert.Contains($"Aucun consultant trouvé avec l'UUID {nonExistentUuid}", exception.Message);
    }
    [Fact]
    public void Test_DeleteConsultant_Should_Remove_Consultant()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context);  // Ajouter des consultants à la base de données en mémoire
        var consultantService = new ConsultantService(context);

        // On prend un consultant existant à partir de la base de données
        var existingConsultant = context.Consultants.First();

        // Act
        consultantService.DeleteConsultant(existingConsultant.ConsultantUuid);

        // Assert
        var deletedConsultant = context.Consultants
            .SingleOrDefault(c => c.ConsultantUuid == existingConsultant.ConsultantUuid);

        Assert.Null(deletedConsultant);
    }

    [Fact]
    public void Test_DeleteConsultant_Should_Throw_Exception_When_Consultant_Not_Found()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context); 
        var consultantService = new ConsultantService(context);


        var nonExistentUuid = Guid.NewGuid();

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => consultantService.DeleteConsultant(nonExistentUuid));

        // Vérifier que l'exception contient le bon message
        Assert.Contains($"Aucun consultant trouvé avec l'UUID : {nonExistentUuid}", exception.Message);
    }
        [Fact]
    public void Test_UpdateConsultant_Should_Throw_Exception_When_Consultant_Not_Found()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var consultantService = new ConsultantService(context);

        var nonExistentUuid = Guid.NewGuid();
        var updatedConsultant = new Consultant
        {
            ConsultantUuid = nonExistentUuid,
            Name = "Nonexistent Consultant",
            Surname = "Test",
            Email = "nonexistent@example.com",
            Phone = "1234567890",
            AvailabilityDate = new DateTime(2024, 5, 10),
            ExpirationDateCI = new DateTime(2025, 5, 10),
            Intern = true,
            Picture = "http://localhost/nonexistent.jpg",
            CopyCI = "http://localhost/nonexistent_copy.jpg",
            enterprise = Enterprise.O2do,
            Profiles = new List<Profile>()
        };

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => consultantService.UpdateConsultant(updatedConsultant));
        Assert.Contains($"Le consultant avec UUID {nonExistentUuid} n'existe pas.", exception.Message);
    }

    [Fact]
    public void Test_UpdateConsultant_Should_Update_Consultant()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var consultantService = new ConsultantService(context);

        // Consultant à mettre à jour
        var existingConsultant = context.Consultants.First();
        var updatedConsultant = new Consultant
        {
            ConsultantUuid = existingConsultant.ConsultantUuid,
            Name = "jean Updated",
            Surname = "Johnson Updated",
            Email = "jean.updated@example.com",
            Phone = "1234567890",
            AvailabilityDate = new DateTime(2024, 5, 10),
            ExpirationDateCI = new DateTime(2025, 5, 10),
            Intern = true,
            Picture = "http://localhost/updated.jpg",
            CopyCI = "http://localhost/updated_copy.jpg",
            enterprise = Enterprise.O2do,
            Profiles = new List<Profile>()
        };

        // Act
        var result = consultantService.UpdateConsultant(updatedConsultant);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("jean Updated", result.Name);
        Assert.Equal("Johnson Updated", result.Surname);
        Assert.Equal("jean.updated@example.com", result.Email);
        Assert.Equal("1234567890", result.Phone);
        Assert.Equal(new DateTime(2024, 5, 10), result.AvailabilityDate);
        Assert.Equal(new DateTime(2025, 5, 10), result.ExpirationDateCI);
        Assert.Equal("http://localhost/updated.jpg", result.Picture);
        Assert.Equal("http://localhost/updated_copy.jpg", result.CopyCI);
        Assert.True(result.Intern);
    }

}
