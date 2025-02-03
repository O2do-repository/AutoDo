var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

var appInsightsConnectionString = Environment.GetEnvironmentVariable("APPINSIGHTS_KEY");

// activer Application Insights
if (!string.IsNullOrEmpty(appInsightsConnectionString))
{
    builder.Services.AddApplicationInsightsTelemetry(options =>
    {
        options.ConnectionString = appInsightsConnectionString;
    });
}

var app = builder.Build();

app.MapGet("/", () => "Hello AutoDo, Lucien and Eric are the best ! ");


app.MapGet("/throw", (ILogger<Program> logger) =>
{
    logger.LogError("Une exception de test est sur le point d'être déclenchée.");
    throw new Exception("Ceci est une exception de test pour Log Stream.");
});

app.Run();