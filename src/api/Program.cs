
using System.Security.Claims;
using System.Text.Json;
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
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IEnterpriseService, EnterpriseService>();
builder.Services.AddScoped<IKeywordService, KeywordService>();
builder.Services.AddScoped<IGitHubService, GitHubService>();



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

app.Use(async (context, next) =>
{
    var encodedPrincipal = context.Request.Headers["X-MS-CLIENT-PRINCIPAL"].SingleOrDefault();

    if (!string.IsNullOrEmpty(encodedPrincipal))
    {
        var decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encodedPrincipal));
        var principalData = JsonSerializer.Deserialize<AzureUserPrincipal>(decoded);

        var claims = principalData.Claims.Select(c => new Claim(c.Type, c.Value));
        context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "AzureAuth"));
    }

    await next();
});
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.UseCors("all");

app.Run();
