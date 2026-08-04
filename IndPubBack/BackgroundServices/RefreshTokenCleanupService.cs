using IndPubBack.Repositories.Interfaces;

namespace IndPubBack.BackgroundServices;

public class RefreshTokenCleanupService(
    ILogger<RefreshTokenCleanupService> logger, 
    IServiceScopeFactory scopeFactory, 
    IConfiguration configuration) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("RefreshTokenCleanupService is starting.");

        await RunCleanupAsync(stoppingToken);

        var hours = configuration.GetValue<int>("TokenCleanup:IntervalHours");
        using var timer = new PeriodicTimer(TimeSpan.FromHours(hours));
        
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await RunCleanupAsync(stoppingToken);
        }
    }

    internal async Task RunCleanupAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var repo = scope.ServiceProvider.GetRequiredService<IRefreshTokenRepository>();

            await repo.RemoveOldTokensAsync(cancellationToken);
            logger.LogInformation("Old refresh tokens removed.");
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Expected during shutdown — rethrow without re-logging, let the host handle it.
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to clean refresh tokens.");
        }
    }
}