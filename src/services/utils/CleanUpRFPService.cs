using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

public class CleanUpRFPService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CleanUpRFPService> _logger;

    public CleanUpRFPService(IServiceProvider serviceProvider, ILogger<CleanUpRFPService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();

            var rfpService = scope.ServiceProvider.GetRequiredService<IRfpService>();

            try
            {
                rfpService.DeleteOldRFPs();
                _logger.LogInformation("Suppression des RFP expirées effectuée avec succès.");
            }
            catch (InvalidOperationException)
            {
                _logger.LogDebug("Aucune RFP expirée à supprimer ce jour.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression des RFP expirées.");
                throw;
            }

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
}
