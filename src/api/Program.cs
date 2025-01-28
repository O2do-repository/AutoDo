var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello AutoDo, Eric is the best !");

app.MapGet("/generate-error", (ILogger<Program> logger) =>
{
    try
    {
        // Force une exception volontaire
        throw new Exception("Ceci est une exception de test !");
    }
    catch (Exception ex)
    {
        // Loguer l'exception avec sa pile d'appels
        logger.LogError(ex, "Une erreur est survenue");

        // Retourner une réponse HTTP 500 avec un message simple
        return Results.Problem(
            title: "Une erreur interne est survenue.",
            detail: ex.Message,
            statusCode: 500
        );
    }
});
app.Run();
