using Azure.Identity;
using Azure.Monitor.Ingestion;
public class AzureMonitorLoggerProvider : ILoggerProvider
{
    private readonly LogsIngestionClient _client;
    private readonly string _dcrImmutableId;
    private readonly string _tableName;

    public AzureMonitorLoggerProvider()
    {
        var dceUrl = Environment.GetEnvironmentVariable("AZURE_MONITOR_DCE");
        _dcrImmutableId = Environment.GetEnvironmentVariable("AZURE_MONITOR_DCR_ID");
        _tableName = Environment.GetEnvironmentVariable("AZURE_MONITOR_TABLE");

        if (string.IsNullOrEmpty(dceUrl) || string.IsNullOrEmpty(_dcrImmutableId) || string.IsNullOrEmpty(_tableName))
        {
            throw new InvalidOperationException("Les variables d'environnement Azure Monitor ne sont pas configurées.");
        }

        // Récupérer les credentials Azure à partir des variables d'environnement
        var tenantId = Environment.GetEnvironmentVariable("AZURE_TENANT_ID");
        var clientId = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID");
        var clientSecret = Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET");

        if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
        {
            throw new InvalidOperationException("Les variables d'environnement Azure ne sont pas configurées.");
        }

        var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
        
        try
        {
            var token = credential.GetToken(new Azure.Core.TokenRequestContext(new[] { "https://monitor.azure.com/.default" }));
            Console.WriteLine("Authentification réussie !");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur d'authentification : {ex.Message}");
        }

        _client = new LogsIngestionClient(new Uri(dceUrl), credential);
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new AzureMonitorLogger(_client, _dcrImmutableId, _tableName, categoryName);
    }

    public void Dispose() { }
}


