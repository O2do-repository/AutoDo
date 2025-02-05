var builder = WebApplication.CreateBuilder(args);

// Ajouter Azure Monitor Logger
builder.Logging.ClearProviders();
builder.Logging.AddProvider(new AzureMonitorLoggerProvider());

var app = builder.Build();

// Route de test
app.MapGet("/", () => "Hello AutoDo, Lucien and Eric are the best  !");

// Simuler une exception
app.MapGet("/throw", (ILogger<Program> logger) =>
{
    logger.LogError("Une exception de test est sur le point d'être déclenchée.");
    throw new Exception("Ceci est une exception de test pour Log Stream.");
});

app.Run();
