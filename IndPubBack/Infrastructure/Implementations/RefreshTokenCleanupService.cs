using IndPubBack.Repositories.Interfaces;

namespace IndPubBack.Infrastructure.Implementations;

public class RefreshTokenCleanupService(ILogger<RefreshTokenCleanupService> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("RefreshTokenCleanupService is starting.");

        var hours = configuration.GetValue<int>("TokenCleanup:IntervalHours");
        using var timer = new PeriodicTimer(TimeSpan.FromHours(hours));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await RunCleanupAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException ex) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation(ex, "RefreshTokenCleanupService is stopping.");
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
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to clean refresh tokens.");
        }
    }
}