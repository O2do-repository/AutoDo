using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;


public class KeywordServiceTests
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
        context.Keywords.AddRange(new List<Keyword>
        {
            new Keyword
            {
                KeywordUuid = new Guid("123e4567-e89b-12d3-a456-426614174000"),
                Name = "Java"
            },
            new Keyword
            {
                KeywordUuid = new Guid("987e4567-e89b-12d3-a456-426614174111"),
                Name = "C#"
            }
        });
        context.SaveChanges();
    }

    [Fact]
    public void Test_GetAllKeywords_Should_Return_Keywords()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var keywordService = new KeywordService(context);

        // Act
        var result = keywordService.GetAllKeywords();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<Keyword>>(result);
        Assert.Equal(2, result.Count);  // Assuming 2 keywords are seeded
    }

    [Fact]
    public void Test_AddKeyword_Should_Add_Keyword()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var keywordService = new KeywordService(context);
        var newKeyword = new Keyword
        {
            Name = "Python"
        };

        // Act
        var result = keywordService.AddKeyword(newKeyword);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Python", result.Name);
        Assert.NotEqual(Guid.Empty, result.KeywordUuid);  // Should generate a new GUID
    }

    [Fact]
    public void Test_DeleteKeyword_Should_Remove_Keyword_Successfully()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var keywordService = new KeywordService(context);
        var keywordUuid = new Guid("123e4567-e89b-12d3-a456-426614174000");

        // Act
        keywordService.DeleteKeyword(keywordUuid);

        // Assert
        var keyword = context.Keywords.FirstOrDefault(k => k.KeywordUuid == keywordUuid);
        Assert.Null(keyword); // Should be null after deletion
    }

    [Fact]
    public void Test_DeleteKeyword_Should_Throw_Exception_When_Keyword_Not_Found()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var keywordService = new KeywordService(context);
        var nonExistentUuid = Guid.NewGuid();  // UUID that does not exist in the database

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => keywordService.DeleteKeyword(nonExistentUuid));
        Assert.Contains("Le mot-clé avec UUID", exception.Message); // Check for specific error message
    }
}

