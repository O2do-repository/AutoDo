
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

var apiKey = configuration["ApiKey:SecretKey"];
if (string.IsNullOrEmpty(apiKey))
{
    throw new Exception("API Key manquante dans la configuration !");
}

builder.Services.AddDbContext<AutoDoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

builder.Services.AddHostedService<CleanUpRFPService>();

builder.Services.AddScoped<IRfpService, RfpService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IMatchingService, MatchingService>();
builder.Services.AddScoped<IConsultantService, ConsultantService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IEnterpriseService, EnterpriseService>();
builder.Services.AddScoped<IKeywordService, KeywordService>();
builder.Services.AddScoped<IMatchingFeedbackService, MatchingFeedbackService>();


// Translator service
builder.Services.Configure<AzureTranslatorOptions>(
    builder.Configuration.GetSection("AzureTranslator"));
builder.Services.AddScoped<ITranslationService, TranslationService>();


builder.Services.AddDbContext<AutoDoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



// AI service
builder.Services.AddHttpClient();

// 2. Récupération des valeurs depuis appsettings.json
var infomaniakApiKey = builder.Configuration["Infomaniak:ApiKey"];
var infomaniakModel = builder.Configuration["Infomaniak:Model"]; 
var infomaniakProductId = builder.Configuration["Infomaniak:ProductId"];

if (string.IsNullOrEmpty(infomaniakApiKey))
{
    throw new Exception("API Key manquante dans la configuration !");
}


builder.Services.AddScoped<IAiNormalizationService>(sp =>
{
    var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient();
    return new AiNormalizationService(httpClient, infomaniakApiKey, infomaniakModel, infomaniakProductId);
});
    

string[] allowedOrigins =
{
    "https://o2do-repository.github.io",
    "http://localhost:3000",              
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

app.Use(async (context, next) =>
{
    // Exclure la route de validation
    if (context.Request.Path.StartsWithSegments("/api/validate-key"))
    {
        await next();
        return;
    }

    var apiKeyHeader = context.Request.Headers["X-API-KEY"].SingleOrDefault();
    var configuredApiKey = app.Configuration["ApiKey:SecretKey"];
    if (string.IsNullOrEmpty(apiKeyHeader) || apiKeyHeader != configuredApiKey)
    {
        context.Response.StatusCode = 401; // Unauthorized
        await context.Response.WriteAsync("Clé API invalide ou manquante.");
        return;
    }
    
    await next();
});

app.UseHttpsRedirection();
app.UseRouting();

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

Console.WriteLine(">>> TEST DE CONNEXION IA EN COURS...");

using (var scope = app.Services.CreateScope())
{
    var aiService = scope.ServiceProvider.GetRequiredService<IAiNormalizationService>();
    
    try 
    {
        // On appelle l'IA avec des données de test simples
        var testResult = await aiService.NormalizeAsync(
            "Développeur C#", 
            new List<string> { "C#", ".NET", "Azure" }, 
            new List<string> { "Agile" }
        );

        Console.WriteLine(">>> SUCCÈS ! L'IA a répondu :");
        Console.WriteLine($"   Titre : {testResult.NormalizedJobTitle}");
        Console.WriteLine($"   Skills : {string.Join(", ", testResult.NormalizedSkills)}");
        Console.WriteLine($"   Keywords : {string.Join(", ", testResult.NormalizedKeywords)}");
    }
    catch (Exception ex)
    {
        Console.WriteLine(">>> ÉCHEC CRITIQUE DE L'IA :");
        Console.WriteLine($"   Message : {ex.Message}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"   Détail : {ex.InnerException.Message}");
            Console.WriteLine($"   Stack : {ex.InnerException.StackTrace}");
        }
        // On ne relance pas pour ne pas bloquer le serveur, mais on affiche tout
    }
}

app.Run();
