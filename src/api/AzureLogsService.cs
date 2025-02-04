

using Azure.Identity;
using Azure.Monitor.Ingestion;

public class AzureLogService
{
    private readonly LogsIngestionClient _client;
    private readonly string _dcrImmutableId;
    private readonly string _tableName;

    public AzureLogService()
    {
        var dceUrl = Environment.GetEnvironmentVariable("AZURE_MONITOR_DCE");
        _dcrImmutableId = Environment.GetEnvironmentVariable("AZURE_MONITOR_DCR_ID");
        _tableName = Environment.GetEnvironmentVariable("AZURE_MONITOR_TABLE");

        if (string.IsNullOrEmpty(dceUrl) || string.IsNullOrEmpty(_dcrImmutableId) || string.IsNullOrEmpty(_tableName))
        {
            throw new InvalidOperationException("Les variables d'environnement Azure Monitor ne sont pas configurées.");
        }

        var credential = new DefaultAzureCredential();
        _client = new LogsIngestionClient(new Uri(dceUrl), credential);
    }

    public async Task SendLogAsync(object logEntry)
    {
        try
        {
            var logs = new List<object> { logEntry };
            await _client.UploadAsync(_dcrImmutableId, _tableName, logs);
            Console.WriteLine("Log envoyé avec succès !");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur d'envoi du log : {ex.Message}");
        }
    }
}
