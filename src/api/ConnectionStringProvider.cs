namespace Api;

public class ConnectionStringProvider : IConnectionStringProvider{
    private readonly IConfiguration _configuration;

    public ConnectionStringProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string Get(string key)
    {
        // Détection simple d'Azure
        bool isAzure = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME"));
        
        if (isAzure)
        {
            string dataPath = Path.Combine(Environment.GetEnvironmentVariable("HOME"), "data");
            Directory.CreateDirectory(dataPath); // Crée le dossier s'il n'existe pas
            return $"Data Source={Path.Combine(dataPath, "AutoDo.db")}";
        }
        
        // En local, utiliser la configuration
        return _configuration.GetConnectionString(key);
    }
}