var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();
var app = builder.Build();

// Route de base
app.MapGet("/", () => "Hello AutoDo, Eric is the best !");

app.MapGet("/throw", (ILogger<Program> logger) =>
{
    logger.LogError("Une exception de test est sur le point d'être déclenchée.");
    throw new Exception("Ceci est une exception de test pour Log Stream.");
});

app.Run();
