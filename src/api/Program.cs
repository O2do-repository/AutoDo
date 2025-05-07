
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAuthorization();




builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();


var configuration = builder.Configuration;

// Lecture de la clé API depuis la configuration
var apiKey = configuration["ApiKey:SecretKey"];
if (string.IsNullOrEmpty(apiKey))
{
    throw new Exception("API Key manquante dans la configuration !");
}

builder.Services.AddDbContext<AutoDoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

builder.Services.AddScoped<IRfpService, RfpService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IMatchingService, MatchingService>();
builder.Services.AddScoped<IConsultantService, ConsultantService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IEnterpriseService, EnterpriseService>();
builder.Services.AddScoped<IKeywordService, KeywordService>();


builder.Services.AddDbContext<AutoDoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    

// Ajouter CORS
// CORS: autoriser frontend prod et dev
string[] allowedOrigins =
{
    "https://o2do-repository.github.io",
    "http://localhost:3000",             // développement (Vite)            
};

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});


var app = builder.Build();

app.UseCors("FrontendPolicy");

app.MapGet("/", () => "Hello AutoDo, Test feature branch");

// app.Use(async (context, next) =>
// {
//     if (context.Request.Method == "OPTIONS")
//     {
//         context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
//         context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
//         context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, X-API-KEY");

//         context.Response.StatusCode = 204; // No Content
//         return;
//     }

//     await next();
// });

app.Use(async (context, next) =>
{
    // Exclure la route de validation de la clé API de la vérification
    if (context.Request.Path.StartsWithSegments("/api/validate-key"))
    {
        await next();
        return;
    }

    var apiKeyHeader = context.Request.Headers["X-API-KEY"].SingleOrDefault();
    Console.WriteLine(apiKeyHeader);
    var configuredApiKey = app.Configuration["ApiKey:SecretKey"];
    Console.WriteLine(configuredApiKey);
    if (string.IsNullOrEmpty(apiKeyHeader) || apiKeyHeader != configuredApiKey)
    {
        context.Response.StatusCode = 401; // Unauthorized
        await context.Response.WriteAsync("Clé API invalide ou manquante.");
        return;
    }
    
    await next();
});



// Middleware de base
app.UseHttpsRedirection();
app.UseRouting();

// Middleware d’authentification et d’autorisation
app.UseAuthentication();

app.UseAuthorization();




app.MapControllers();

app.MapPost("/api/validate-key", async (HttpContext context) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();
    var jsonData = JsonSerializer.Deserialize<Dictionary<string, string>>(body);
    
    if (jsonData != null && jsonData.TryGetValue("apiKey", out var providedApiKey))
    {
        var configuredApiKey = app.Configuration["ApiKey:SecretKey"];
        
        if (providedApiKey == configuredApiKey)
        {
            return Results.Ok(new { valid = true });
        }
    }
    
    return Results.Ok(new { valid = false });
});


app.Run();
