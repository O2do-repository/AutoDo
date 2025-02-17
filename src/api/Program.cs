using models;

var builder = WebApplication.CreateBuilder(args);

// Ajouter CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("all", builder =>
    {
        builder.SetIsOriginAllowed(_ => true)  // Allow any origin
               .AllowAnyMethod()              // Allow any HTTP method
               .AllowAnyHeader()              // Allow any headers
               .AllowCredentials();           // Allow credentials
    });
});


var app = builder.Build();

// Créer une instance de RfpServices
var rfpServices = new RfpServices();

app.MapGet("/rfp", () =>
{
    // Récupérer la liste de RFPs
    var receivedDataList = DummyRfpData.GetDummyRfpList();
    var rfpList = receivedDataList.Select(data => data.ToRFP()).ToList();

    // Filtrer les RFPs en fonction de la date limite
    var filteredRfps = rfpServices.FilterRfpDeadlineNotReachedYet(rfpList);

    // Retourner la liste filtrée en JSON
    return Results.Json(filteredRfps);
});


app.MapGet("/", () => "Hello AutoDo, Test feature branch");

app.MapGet("/log", () => new AzureMonitorLoggerConnection().CreateLogger("test"

).LogCritical("cool"));


app.UseCors("all");

app.Run();
