
using models;
using Moq;
using Xunit;

    public class ProfilTest
    {
        private readonly IProfilService _profilService;
        private readonly Mock<IProfilService> _mockService;

        public ProfilTest()
        {
            _mockService = new Mock<IProfilService>();
        }
    

    [Fact]
    public void Test_GetAllProfils_Should_Return_Profils()
    {
        // Arrange
        var fakeProfils = new List<Profil>
        {
            new Profil { Uuid = "1", Ratehour = 50, CV = "https://example.com/cv1.pdf", CV_Date = new DateTime(2023, 5, 1), Job_title = "Software Engineer", Experience_level = Experience.Junior, Skills = new List<string> { Skill.Java, Skill.JavaScript }, Keywords = new List<string> { Keyword.Architect, Keyword.DevOps } }
        };

        _mockService.Setup(s => s.GetAllProfils()).Returns(fakeProfils);

        // Act
        var result = _mockService.Object.GetAllProfils();

        // Assert
        Assert.IsType<List<Profil>>(result); 
        Assert.NotEmpty(result);
    }

    [Fact]
    public void Test_GetAllProfils_Profil_Should_Contain_All_Attributes()
    {
        // Arrange
        var fakeProfils = new List<Profil>
        {
            new Profil { Uuid = "1", Ratehour = 50, CV = "https://example.com/cv1.pdf", CV_Date = new DateTime(2023, 5, 1), Job_title = "Software Engineer", Experience_level = Experience.Junior, Skills = new List<string> { Skill.Java, Skill.JavaScript }, Keywords = new List<string> { Keyword.Architect, Keyword.DevOps } }
        };
        
        _mockService.Setup(s => s.GetAllProfils()).Returns(fakeProfils);

        // Act
        var result = _mockService.Object.GetAllProfils();
        
        // Vérifie qu'on obtient un seul profil de la liste
        var profil = result[0];

        // Assert
        Assert.NotNull(profil.Uuid);
        Assert.NotNull(profil.Ratehour);
        Assert.NotNull(profil.CV);
        Assert.NotNull(profil.CV_Date);
        Assert.NotNull(profil.Job_title);
        Assert.NotNull(profil.Experience_level);
        Assert.NotNull(profil.Skills); 
        Assert.NotNull(profil.Keywords); 
    }


    
    [Fact]
    public void Test_GetAllProfils_Profil_Should_be_equals_to_Profil()
    {
        // Arrange
        var fakeProfils = new List<Profil>
        {
            new Profil { Uuid = "123e4567-e89b-12d3-a456-426614174000", Ratehour = 50, CV = "https://example.com/cv1.pdf", CV_Date = new DateTime(2023, 5, 1), Job_title = "Software Engineer", Experience_level = Experience.Junior, Skills = new List<string> { Skill.Java, Skill.JavaScript }, Keywords = new List<string> { Keyword.Architect, Keyword.DevOps } }
        };
        
        _mockService.Setup(s => s.GetAllProfils()).Returns(fakeProfils);

        // Act
        var result = _mockService.Object.GetAllProfils();

        // Vérifie qu'on obtient un seul profil de la liste
        var profil = result[0]; // On choisit le premier profil ici (index 0)

        // Assert
        Assert.Equal("123e4567-e89b-12d3-a456-426614174000", profil.Uuid);
        Assert.Equal(50, profil.Ratehour);
        Assert.Equal("https://example.com/cv1.pdf", profil.CV);
        Assert.Equal(new DateTime(2023, 5, 1), profil.CV_Date);
        Assert.Equal("Software Engineer", profil.Job_title);
        Assert.Equal(Experience.Junior, profil.Experience_level);
        Assert.Contains(Skill.Java, profil.Skills);
        Assert.Contains(Skill.JavaScript, profil.Skills);
        Assert.Contains(Keyword.Architect, profil.Keywords);
        Assert.Contains(Keyword.DevOps, profil.Keywords);
    }
}
