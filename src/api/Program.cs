var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello AutoDo, Eric is the best !");

app.MapGet("/generate-error", (ILogger<Program> logger) =>
{
    try
    {
        throw new Exception("Ceci est une exception de test !");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Une erreur est survenue");

        return Results.StatusCode(500);
    }
});
app.Run();
