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



// API /rfp avec données
app.MapGet("/rfp",  () =>
{
  var receivedDataList = DummyRfpData.GetDummyRfpList();


    var filteredDataList = receivedDataList
        .Where(data => data.ResponseDate.UtcDateTime > DateTime.UtcNow) // Deadline non expirée
        .ToList();


    // Mapper la liste vers une liste de RFP
    var rfpList = filteredDataList.Select(data => data.ToRFP()).ToList();

    // Retourner la liste en JSON
    return Results.Json(rfpList);
});

// Route de test
app.MapGet("/", () => "Hello AutoDo, Test feature branch");

app.UseCors("all");

app.Run();
