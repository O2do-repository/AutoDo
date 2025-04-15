
using Microsoft.EntityFrameworkCore;

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

 builder.Services.AddDbContext<AutoDoDbContext>(options =>
      options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

builder.Services.AddScoped<IRfpService, RfpService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IMatchingService, MatchingService>();
builder.Services.AddScoped<IConsultantService, ConsultantService>();

//builder.Services.AddScoped<AutoDoDbContext>();



// Charger la configuration
var configuration = builder.Configuration;

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddDbContext<AutoDoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();


app.MapGet("/", () => "Hello AutoDo, Test feature branch");



app.MapControllers();

app.UseCors("all");

app.Run();
