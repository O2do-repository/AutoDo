using Polly;
using Polly.Retry;
using Microsoft.Extensions.Logging;

public static class RetryService
{
    public static async Task<T> RetryAsync<T>(
        Func<Task<T>> operation,
        int maxAttempts = 3,
        Func<T, bool>? resultShouldRetry = null,
        ILogger? logger = null)
    {
        var retryPolicy = Policy
            .Handle<Exception>()
            .OrResult<T>(result => resultShouldRetry != null && resultShouldRetry(result))
            .WaitAndRetryAsync(
                retryCount: maxAttempts,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt - 1)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    logger?.LogWarning($"Retry {retryCount}/{maxAttempts} after {timespan.TotalSeconds}s due to: {outcome.Exception?.Message ?? "non-success result"}");
                });

        return await retryPolicy.ExecuteAsync(operation);
         
    }
}
