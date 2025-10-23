using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

public class SkillServiceTests
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
        context.Skills.AddRange(new List<Skill>
        {
            new Skill
            {
                SkillUuid = new Guid("123e4567-e89b-12d3-a456-426614174000"),
                Name = "Java",
                NameFr = "Java",
                NameNl = "Java",
                NameEn = "Java"
            },
            new Skill
            {
                SkillUuid = new Guid("987e4567-e89b-12d3-a456-426614174111"),
                Name = "C#",
                NameFr = "C#",
                NameNl = "C#",
                NameEn = "C#"
            }
        });
        context.SaveChanges();
    }

    [Fact]
    public void Test_GetAllSkills_Should_Return_Skills()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var mockTranslationService = new Mock<ITranslationService>();
        var skillService = new SkillService(context, mockTranslationService.Object);

        // Act
        var result = skillService.GetAllSkills();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<Skill>>(result);
        Assert.Equal(2, result.Count);  // Assuming 2 skills are seeded
    }

    [Fact]
    public async Task Test_AddSkill_Should_Add_Skill()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var mockTranslationService = new Mock<ITranslationService>();

        // Simuler les traductions (mock)
        mockTranslationService.Setup(t => t.TranslateTextAsync("Python", "en")).ReturnsAsync("Python");
        mockTranslationService.Setup(t => t.TranslateTextAsync("Python", "fr")).ReturnsAsync("Python");
        mockTranslationService.Setup(t => t.TranslateTextAsync("Python", "nl")).ReturnsAsync("Python");

        var skillService = new SkillService(context, mockTranslationService.Object);
        var newSkill = new Skill
        {
            Name = "Python"
        };

        // Act
        var result = await skillService.AddSkill(newSkill);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Python", result.Name);
        Assert.Equal("Python", result.NameEn);
        Assert.Equal("Python", result.NameFr);
        Assert.Equal("Python", result.NameNl);
        Assert.NotEqual(Guid.Empty, result.SkillUuid);
    }


    [Fact]
    public void Test_DeleteSkill_Should_Remove_Skill_Successfully()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var mockTranslationService = new Mock<ITranslationService>();
        var skillService = new SkillService(context, mockTranslationService.Object);
        var skillUuid = new Guid("123e4567-e89b-12d3-a456-426614174000");

        // Act
        skillService.DeleteSkill(skillUuid);

        // Assert
        var skill = context.Skills.FirstOrDefault(s => s.SkillUuid == skillUuid);
        Assert.Null(skill); // Should be null after deletion
    }

    [Fact]
    public void Test_DeleteSkill_Should_Throw_Exception_When_Skill_Not_Found()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var mockTranslationService = new Mock<ITranslationService>();
        var skillService = new SkillService(context, mockTranslationService.Object);
        var nonExistentUuid = Guid.NewGuid();  // UUID qui n'existe pas dans la base de données

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => skillService.DeleteSkill(nonExistentUuid));  // Utiliser skillService
        Assert.Contains("Le skill avec UUID", exception.Message); // Vérifier le message d'erreur spécifique
    }

}

