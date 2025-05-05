
using System.Security.Claims;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// Ajouter CORS
// CORS: autoriser frontend prod et dev
string[] allowedOrigins =
{
    "https://o2do-repository.github.io", // production (GitHub Pages)
    "http://localhost:3000/AutoDo/",             // développement (Vite)            
};

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});


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

// Middleware de base
app.UseHttpsRedirection();
app.UseRouting();

// Middleware d’authentification et d’autorisation
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();


app.UseCors("FrontendPolicy");

app.Run();
