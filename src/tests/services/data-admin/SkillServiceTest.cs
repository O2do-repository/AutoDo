using Microsoft.EntityFrameworkCore;
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
                Name = "Java"
            },
            new Skill
            {
                SkillUuid = new Guid("987e4567-e89b-12d3-a456-426614174111"),
                Name = "C#"
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
        var skillService = new SkillService(context);

        // Act
        var result = skillService.GetAllSkills();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<Skill>>(result);
        Assert.Equal(2, result.Count);  // Assuming 2 skills are seeded
    }

    [Fact]
    public void Test_AddSkill_Should_Add_Skill()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var skillService = new SkillService(context);
        var newSkill = new Skill
        {
            Name = "Python"
        };

        // Act
        var result = skillService.AddSkill(newSkill);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Python", result.Name);
        Assert.NotEqual(Guid.Empty, result.SkillUuid);  // Should generate a new GUID
    }

    [Fact]
    public void Test_DeleteSkill_Should_Remove_Skill_Successfully()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var skillService = new SkillService(context);
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
        var skillService = new SkillService(context);  
        var nonExistentUuid = Guid.NewGuid();  // UUID qui n'existe pas dans la base de données

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => skillService.DeleteSkill(nonExistentUuid));  // Utiliser skillService
        Assert.Contains("Le skill avec UUID", exception.Message); // Vérifier le message d'erreur spécifique
    }

}

