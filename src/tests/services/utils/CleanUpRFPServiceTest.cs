using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class CleanUpRFPServiceTests
{
    [Fact]
    public async Task Test_ExecuteAsync_Calls_DeleteOldRFPs()
    {
        // Arrange
        var mockRfpService = new Mock<IRfpService>();
        var services = new ServiceCollection()
            .AddScoped(_ => mockRfpService.Object)
            .BuildServiceProvider();

        var logger = Mock.Of<ILogger<CleanUpRFPService>>();
        var service = new CleanUpRFPService(services, logger);
        var cts = new CancellationTokenSource();

        // Act
        var task = Task.Run(async () =>
        {
            await service.StartAsync(cts.Token);
            await Task.Delay(100);
            cts.Cancel();
            await service.StopAsync(CancellationToken.None);
        });

        await task;

        // Assert
        mockRfpService.Verify(s => s.DeleteOldRFPs(), Times.Once);
    }


    [Fact]
    public async Task Test_ExecuteAsync_When_InvalidOperationException_DoesNotThrow()
    {
        var mockRfpService = new Mock<IRfpService>();
        mockRfpService.Setup(s => s.DeleteOldRFPs()).Throws<InvalidOperationException>();

        var services = new ServiceCollection()
            .AddScoped(_ => mockRfpService.Object)
            .BuildServiceProvider();

        var logger = Mock.Of<ILogger<CleanUpRFPService>>();
        var service = new CleanUpRFPService(services, logger);
        var cts = new CancellationTokenSource();

        // Act & Assert
        var task = Task.Run(async () =>
        {
            await service.StartAsync(cts.Token);
            await Task.Delay(100);
            cts.Cancel();
            await service.StopAsync(CancellationToken.None);
        });

        var ex = await Record.ExceptionAsync(() => task);
        Assert.Null(ex); // l'exception est capturée dans le service
    }

    [Fact]
    public async Task Test_ExecuteAsync_When_Exception_IsThrown()
    {
        var mockRfpService = new Mock<IRfpService>();
        mockRfpService.Setup(s => s.DeleteOldRFPs()).Throws(new Exception("Boom"));

        var services = new ServiceCollection()
            .AddScoped(_ => mockRfpService.Object)
            .BuildServiceProvider();

        var logger = Mock.Of<ILogger<CleanUpRFPService>>();
        var service = new CleanUpRFPService(services, logger);
        var cts = new CancellationTokenSource();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () =>
        {
            await service.StartAsync(cts.Token);
            await Task.Delay(100);
            cts.Cancel();
            await service.StopAsync(CancellationToken.None);
        });
    }

}
