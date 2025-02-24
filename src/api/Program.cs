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

builder.Services.AddControllers();

builder.Services.AddScoped<IRfpService, RfpService>();
builder.Services.AddScoped<IProfilService, ProfilService>();

var app = builder.Build();

app.MapGet("/", () => "Hello AutoDo, Test feature branch");

app.MapGet("/log", () => new AzureMonitorLoggerConnection().CreateLogger("test"

).LogCritical("cool"));


app.MapControllers();

app.UseCors("all");

app.Run();
