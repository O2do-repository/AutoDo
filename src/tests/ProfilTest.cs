
    using models;
    using Xunit;

    public class ProfilTest
    {
        private readonly IProfilService _profilService;

        public ProfilTest()
        {
            _profilService = new ProfilService();
            
        }

    [Fact]
    public void Test_GetAllProfils_Should_Return_Profils ()
    {
        // Arrange
        // Act
        var result = _profilService.GetAllProfils();

        // Assert
        // Vérifie qu'on reçoit une list d'object de type profils
        Assert.IsType<List<Profil>>(result); 
        Assert.NotEmpty(result);
    }

    [Fact]
    public void Test_GetAllProfils_Profil_Should_Contain_All_Attributes()
    {
        // Act
        var result = _profilService.GetAllProfils();

        // Vérifie qu'on obtient un seul profil de la liste
        var profil = result[0]; // On choisit le premier profil ici (index 0)

        // Assert
        Assert.NotNull(profil.Uuid);
        Assert.NotNull(profil.Ratehour);
        Assert.NotNull(profil.CV);
        Assert.NotNull(profil.CV_Date);
        Assert.NotNull(profil.Job_title);
        Assert.NotNull(profil.Experience_level);
        Assert.NotNull(profil.Skills); 
        Assert.NotNull(profil.Keywords); 

        // Vérifie que les listes Skills et Keywords ne sont pas vides
        Assert.NotEmpty(profil.Skills);
        Assert.NotEmpty(profil.Keywords);
    }
    [Fact]
    public void Test_GetAllProfils_Profil_Should_be_equals_to_storeProfil()
    {
        // Act
        var result = _profilService.GetAllProfils();

        // Vérifie qu'on obtient un seul profil de la liste
        var profil = result[0]; // On choisit le premier profil ici (index 0)

        // Assert
        // Comparaison des valeurs de ce profil avec celles attendues des données mockées
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
