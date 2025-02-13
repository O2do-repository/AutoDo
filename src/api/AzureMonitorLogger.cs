using System.Diagnostics;
using Azure.Monitor.Ingestion;

public class AzureMonitorLogger : ILogger
{
    private readonly LogsIngestionClient _client;
    private readonly string _dcrImmutableId;
    private readonly string _tableName;
    private readonly string _categoryName;

    public AzureMonitorLogger(LogsIngestionClient client, string dcrImmutableId, string tableName, string categoryName)
    {
        _client = client;
        _dcrImmutableId = dcrImmutableId;
        _tableName = tableName;
        _categoryName = categoryName;
    }

    public IDisposable? BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public async void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var source = GetSource();

        var logEntry = new
        {
            TimeGenerated = DateTime.UtcNow,
            Level = logLevel.ToString(),
            Category = _categoryName,
            Message = formatter(state, exception),
            Exception = exception?.ToString(),
            Source = source
        };

        try
        {
            var logs = new List<object> { logEntry };
            await _client.UploadAsync(_dcrImmutableId, _tableName, logs);
            Console.WriteLine($"Log envoyé à Azure Monitor : {logEntry.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur d'envoi du log : {ex.Message}");
        }
    }

    private string GetSource()
    {
        var stackTrace = new StackTrace(true);
        var frames = stackTrace.GetFrames();

        if (frames != null)
        {
            foreach (var frame in frames)
            {
                var method = frame.GetMethod();
                if (method != null)
                {
                    return $"{method.DeclaringType?.FullName}.{method.Name}";
                }
            }
        }

        return "Source unknown";
    }
}