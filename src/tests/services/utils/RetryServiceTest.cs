using System;
using System.Threading.Tasks;
using Xunit;

public class RetryServiceTests
{
    [Fact]
    public async Task RetryAsync_ShouldReturnResult_WhenNoExceptionThrown()
    {
        // Arrange
        int attempt = 0;
        Task<int> Operation() => Task.FromResult(++attempt);

        // Act
        var result = await RetryService.RetryAsync(Operation);

        // Assert
        Assert.Equal(1, result); // Un seul appel
    }

    [Fact]
    public async Task RetryAsync_ShouldRetryUntilSuccess()
    {
        // Arrange
        int attempt = 0;
        Task<string> Operation() =>
            Task.FromResult(++attempt < 3 ? throw new Exception("Fail") : "Success");

        // Act
        var result = await RetryService.RetryAsync(Operation, maxAttempts: 5);

        // Assert
        Assert.Equal("Success", result);
        Assert.Equal(3, attempt); // 2 échecs + 1 succès
    }


    [Fact]
    public async Task RetryAsync_ShouldRetryOnBadResult()
    {
        // Arrange
        int attempt = 0;
        Task<int> Operation() => Task.FromResult(++attempt < 3 ? 0 : 42);

        // Retry si le résultat est 0
        bool ResultShouldRetry(int result) => result == 0;

        // Act
        var result = await RetryService.RetryAsync(Operation, resultShouldRetry: ResultShouldRetry);

        // Assert
        Assert.Equal(42, result);
        Assert.Equal(3, attempt);
    }

    [Fact]
    public async Task RetryAsync_ShouldNotRetry_WhenResultIsAcceptable()
    {
        // Arrange
        int attempt = 0;
        Task<int> Operation() => Task.FromResult(++attempt);

        bool ResultShouldRetry(int result) => false;

        // Act
        var result = await RetryService.RetryAsync(Operation, resultShouldRetry: ResultShouldRetry);

        // Assert
        Assert.Equal(1, result); // Pas de retry
        Assert.Equal(1, attempt);
    }
}
