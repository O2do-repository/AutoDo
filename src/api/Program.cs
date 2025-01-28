var builder = WebApplication.CreateBuilder(args);

// Ajout du logging pour capturer les exceptions
builder.Logging.AddConsole(); // Permet de loguer dans la console et Azure Log Stream

var app = builder.Build();

// Route de base
app.MapGet("/", () => "Hello AutoDo, Eric is the best!");

// Middleware pour capturer et loguer les exceptions globales
app.Use(async (context, next) =>
{
    try
    {
        await next(); // Passe au prochain middleware ou endpoint
    }
    catch (Exception ex)
    {
        // Loggue l'exception ici
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Une exception non gérée a été interceptée.");

        // Retourne une réponse personnalisée pour éviter un message générique 500
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("Une erreur interne s'est produite.");
    }
});

// Route pour tester une exception
app.MapGet("/throw", (ILogger<Program> logger) =>
{
    // Logger un message avant de lancer l'exception
    logger.LogError("Une exception de test est sur le point d'être déclenchée.");
    throw new Exception("Ceci est une exception de test pour Log Stream.");
});

app.Run();
