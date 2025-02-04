using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AzureLogService _logService;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, AzureLogService logService, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logService = logService;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext); 
        }
        catch (Exception ex)
        {
            var stackTrace = ex.StackTrace;
            var source = GetSourceFromStackTrace(stackTrace);

            // Create custom logs
            var logEntry = new
            {
                TimeGenerated = DateTime.UtcNow,
                ExceptionType = ex.GetType().ToString(),
                Source = source, // Extrait du stack trace
                Message = ex.Message,
                StackTrace = ex.StackTrace,
                AdditionalInfo = new
                {
                    UserId = httpContext.User?.Identity?.Name ?? "Unknown",
                    RequestId = httpContext.TraceIdentifier,
                    Environment = "Production",
                    ResponseCode = httpContext.Response.StatusCode  // Ajouter le code de statut HTTP dans AdditionalInfo
                }
            };

            // Envoi des logs à Azure Monitor
            await _logService.SendLogAsync(logEntry);

            // Log local pour développement
            _logger.LogError(ex, "Une exception a été capturée et envoyée à Azure Monitor.");

            throw;
        }
    }

    // Méthode pour extraire la source de la pile d'exécution
    private string GetSourceFromStackTrace(string stackTrace)
    {
        if (string.IsNullOrEmpty(stackTrace)) return "Source inconnue";

        var stackLines = stackTrace.Split('\n');
        foreach (var line in stackLines)
        {
            if (line.Contains(" at "))
            {
                var methodDetails = line.Substring(line.IndexOf(" at ") + 4).Trim();
                return methodDetails;
            }
        }
        return "Source inconnue";
    }
}
