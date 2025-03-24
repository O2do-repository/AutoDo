// using Azure.Identity;
// using Azure.Monitor.Ingestion;

// public class AzureMonitorLoggerConnection : ILoggerProvider
// {
//     private readonly LogsIngestionClient _client;
//     private readonly string _dcrImmutableId;
//     private readonly string _tableName;

//     public AzureMonitorLoggerConnection()
//     {
//         // Récupération et validation des variables d'environnement
//         var dceUrl = Environment.GetEnvironmentVariable("AZURE_MONITOR_DCE");
//         _dcrImmutableId = Environment.GetEnvironmentVariable("AZURE_MONITOR_DCR_ID");
//         _tableName = Environment.GetEnvironmentVariable("AZURE_MONITOR_TABLE");

//         if (string.IsNullOrWhiteSpace(dceUrl) || string.IsNullOrWhiteSpace(_dcrImmutableId) || string.IsNullOrWhiteSpace(_tableName))
//         {
//             throw new InvalidOperationException("Les variables d'environnement Azure Monitor ne sont pas configurées.");
//         }

//         // Récupération des credentials Azure
//         var tenantId = Environment.GetEnvironmentVariable("AZURE_TENANT_ID");
//         var clientId = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID");
//         var clientSecret = Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET");

//         if (string.IsNullOrWhiteSpace(tenantId) || string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
//         {
//             throw new InvalidOperationException("Les variables d'environnement Azure ne sont pas configurées.");
//         }

//         // Création de l'objet d'authentification
//         var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);

//         // Test de l'authentification
//         try
//         {
//             credential.GetToken(new Azure.Core.TokenRequestContext(new[] { "https://monitor.azure.com/.default" }));
//         }
//         catch (Exception ex)
//         {
//             throw new InvalidOperationException("Échec de l'authentification auprès d'Azure Monitor.", ex);
//         }

//         // Initialisation du client de journalisation
//         _client = new LogsIngestionClient(new Uri(dceUrl), credential);
//     }

//     public ILogger CreateLogger(string categoryName) =>
//         new AzureMonitorLogger(_client, _dcrImmutableId, _tableName, categoryName);

//     public void Dispose() { }
// }
