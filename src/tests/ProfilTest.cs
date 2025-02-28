using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

public class ProfilTest
{
    private AutoDoDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AutoDoDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Base unique par test
        .Options;

        return new AutoDoDbContext(options);
    }

    private void SeedDatabase(AutoDoDbContext context)
    {
        context.Profils.AddRange(new List<Profil>
        {
            new Profil
            {
                Uuid = "123e4567-e89b-12d3-a456-426614174000",
                Ratehour = 50,
                CV = "https://example.com/cv1.pdf",
                CV_Date = new DateTime(2023, 5, 1),
                Job_title = "Software Engineer",
                Experience_level = Experience.Junior,
                Skills = new List<string> { Skill.Java.ToString(), Skill.JavaScript.ToString() },
                Keywords = new List<string> { Keyword.Architect.ToString(), Keyword.DevOps.ToString() } 
            },
            new Profil
            {
                Uuid = "2",
                Ratehour = 40,
                CV = "https://example.com/cv2.pdf",
                CV_Date = new DateTime(2023, 6, 1),
                Job_title = "Backend Developer",
                Experience_level = Experience.Senior,
                Skills = new List<string> { Skill.JavaScript.ToString() },  
                Keywords = new List<string> { Keyword.DataScience.ToString() } 
            }
        });

        context.SaveChanges();
    }

    [Fact]
    public void Test_GetAllProfils_Should_Return_Profils()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var profilService = new ProfilService(context);

        // Act
        var result = profilService.GetAllProfils();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Test_GetAllProfils_Profil_Should_Contain_All_Attributes()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var profilService = new ProfilService(context);

        // Act
        var result = profilService.GetAllProfils();
        var profil = result.FirstOrDefault();

        // Assert
        Assert.NotNull(profil);
        Assert.NotNull(profil.Uuid);
        Assert.NotEqual(0, profil.Ratehour);
        Assert.NotNull(profil.CV);
        Assert.NotEqual(default, profil.CV_Date); 
        Assert.NotNull(profil.Job_title);
        Assert.NotNull(profil.Experience_level); 
        Assert.NotNull(profil.Skills);
        Assert.NotEmpty(profil.Skills);
        Assert.NotNull(profil.Keywords);
        Assert.NotEmpty(profil.Keywords);
    }

    [Fact]
    public void Test_GetAllProfils_Profil_Should_be_equals_to_storeProfil()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var profilService = new ProfilService(context);

        // Act
        var result = profilService.GetAllProfils();
        var profil = result.FirstOrDefault();

        // Assert
        Assert.NotNull(profil);
        Assert.Equal("123e4567-e89b-12d3-a456-426614174000", profil.Uuid);
        Assert.Equal(50, profil.Ratehour);
        Assert.Equal("https://example.com/cv1.pdf", profil.CV);
        Assert.Equal(new DateTime(2023, 5, 1), profil.CV_Date);
        Assert.Equal("Software Engineer", profil.Job_title);
        Assert.Equal(Experience.Junior, profil.Experience_level);     
        Assert.Contains(Skill.Java.ToString(), profil.Skills);  
        Assert.Contains(Skill.JavaScript.ToString(), profil.Skills); 
        Assert.Contains(Keyword.Architect.ToString(), profil.Keywords);  
        Assert.Contains(Keyword.DevOps.ToString(), profil.Keywords); 
    }
}
