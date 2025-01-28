var builder = WebApplication.CreateBuilder(args);

// Ajout du logging pour capturer les exceptions
builder.Logging.AddConsole(); // Permet de loguer dans la console et Azure Log Stream

var app = builder.Build();

// Route de base
app.MapGet("/", () => "Hello AutoDo, Eric is the best!");

// Route pour tester une exception
app.MapGet("/throw", (ILogger<Program> logger) =>
{
    // Logger un message avant de lancer l'exception
    logger.LogError("Une exception de test est sur le point d'être déclenchée.");
    throw new Exception("Ceci est une exception de test pour Log Stream.");
});

app.Run();
