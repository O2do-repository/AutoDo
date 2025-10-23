using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class KeywordServiceTests
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
        context.Keywords.AddRange(new List<Keyword>
        {
            new Keyword
            {
                KeywordUuid = new Guid("123e4567-e89b-12d3-a456-426614174000"),
                Name = "Java",
                NameFr = "Java",
                NameNl = "Java",
                NameEn = "Java"
            },
            new Keyword
            {
                KeywordUuid = new Guid("987e4567-e89b-12d3-a456-426614174111"),
                Name = "C#",
                NameFr = "C#",
                NameNl = "C#",
                NameEn = "C#"
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
        var mockTranslationService = new Mock<ITranslationService>();
        var keywordService = new KeywordService(context, mockTranslationService.Object);

        // Act
        var result = keywordService.GetAllKeywords();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<Keyword>>(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Test_AddKeyword_Should_Add_Keyword()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var mockTranslationService = new Mock<ITranslationService>();

        // Simuler les traductions
        mockTranslationService.Setup(t => t.TranslateTextAsync("Python", "en")).ReturnsAsync("Python");
        mockTranslationService.Setup(t => t.TranslateTextAsync("Python", "fr")).ReturnsAsync("Python");
        mockTranslationService.Setup(t => t.TranslateTextAsync("Python", "nl")).ReturnsAsync("Python");

        var keywordService = new KeywordService(context, mockTranslationService.Object);
        var newKeyword = new Keyword
        {
            Name = "Python"
        };

        // Act
        var result = await keywordService.AddKeyword(newKeyword);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Python", result.Name);
        Assert.Equal("Python", result.NameFr);
        Assert.Equal("Python", result.NameNl);
        Assert.Equal("Python", result.NameEn);
        Assert.NotEqual(Guid.Empty, result.KeywordUuid);
    }

    [Fact]
    public void Test_DeleteKeyword_Should_Remove_Keyword_Successfully()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var mockTranslationService = new Mock<ITranslationService>();
        var keywordService = new KeywordService(context, mockTranslationService.Object);
        var keywordUuid = new Guid("123e4567-e89b-12d3-a456-426614174000");

        // Act
        keywordService.DeleteKeyword(keywordUuid);

        // Assert
        var keyword = context.Keywords.FirstOrDefault(k => k.KeywordUuid == keywordUuid);
        Assert.Null(keyword);
    }

    [Fact]
    public void Test_DeleteKeyword_Should_Throw_Exception_When_Keyword_Not_Found()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        SeedDatabase(context);
        var mockTranslationService = new Mock<ITranslationService>();
        var keywordService = new KeywordService(context, mockTranslationService.Object);
        var nonExistentUuid = Guid.NewGuid();

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => keywordService.DeleteKeyword(nonExistentUuid));
        Assert.Contains("Le mot-clé avec UUID", exception.Message);
    }
}
