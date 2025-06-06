using Microsoft.EntityFrameworkCore;

public class MatchingFeedbackTest
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

        context.MatchingFeedbacks.AddRange(new List<MatchingFeedback>
        {

            new MatchingFeedback
            {
                MatchingFeedbackUuid = Guid.NewGuid(),
                MatchingUuid = Guid.NewGuid(),
                TotalScore = 85,
                JobTitleScore = 20,
                ExperienceScore = 25,
                SkillsScore = 25,
                LocationScore = 15,
                JobTitleFeedback = "Le titre correspond bien au poste recherché.",
                ExperienceFeedback = "Le candidat a une expérience très pertinente.",
                SkillsFeedback = "Les compétences techniques sont adéquates.",
                LocationFeedback = "Localisation acceptable.",
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow
            },
            new MatchingFeedback
            {
                MatchingFeedbackUuid = Guid.NewGuid(),
                MatchingUuid = Guid.NewGuid(),
                TotalScore = 60,
                JobTitleScore = 15,
                ExperienceScore = 20,
                SkillsScore = 10,
                LocationScore = 15,
                JobTitleFeedback = "Le titre est quelque peu éloigné de celui recherché.",
                ExperienceFeedback = "Expérience correcte mais pas tout à fait alignée.",
                SkillsFeedback = "Compétences partiellement adéquates.",
                LocationFeedback = "Le candidat est dans la même région.",
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow
            },
            new MatchingFeedback
            {
                MatchingFeedbackUuid = Guid.NewGuid(),
                MatchingUuid = Guid.Parse("b4a3f7c5-2d91-41ef-bf5f-5d4f3b1994d8"),
                TotalScore = 40,
                JobTitleScore = 10,
                ExperienceScore = 10,
                SkillsScore = 10,
                LocationScore = 10,
                JobTitleFeedback = "Titre de poste non pertinent.",
                ExperienceFeedback = "Manque d’expérience significative.",
                SkillsFeedback = "Compétences insuffisantes.",
                LocationFeedback = "Localisation incompatible.",
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow
            }
        });

        context.SaveChanges();
    }
    [Fact]
    public void Test_GetMatchingFeedbackByMatchingId_ThrowsException_WhenNotFound()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new MatchingFeedbackService(context);

        // Act & Assert
        var ex = Assert.Throws<Exception>(() => service.GetMatchingFeedbackByMatchingId(Guid.NewGuid()));
        Assert.Equal("Aucun Feedback disponible pour ce matching.", ex.Message);
    }
    
        [Fact]
    public void Test_GetMatchingFeedbackByMatchingId_Should_Return_Correct_Feedback()
    {
        // Arrange
        var context = GetInMemoryDbContext();

        SeedDatabase(context);
        var service = new MatchingFeedbackService(context);

        // Act
        var feedback = service.GetMatchingFeedbackByMatchingId(Guid.Parse("b4a3f7c5-2d91-41ef-bf5f-5d4f3b1994d8"));

        // Assert
        Assert.NotNull(feedback);
        Assert.Equal(Guid.Parse("b4a3f7c5-2d91-41ef-bf5f-5d4f3b1994d8"), feedback.MatchingUuid);
        Assert.Equal(40, feedback.TotalScore);
    }
}