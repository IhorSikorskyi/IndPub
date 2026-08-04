using IndPubBack.Repositories.Interfaces;

namespace IndPubBack.BackgroundServices;

public class UpdateBookRatingService(
    ILogger<UpdateBookRatingService> logger,
    IServiceScopeFactory scopeFactory,
    IConfiguration configuration) : BackgroundService
{
    private readonly TimeSpan _updateInterval = TimeSpan.FromHours(
        configuration.GetValue("UpdateBookRating:IntervalHours", 1));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("UpdateBookRatingService is starting.");

        await RunRatingUpdate(stoppingToken);

        try
        {
            using var timer = new PeriodicTimer(_updateInterval);
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await RunRatingUpdate(stoppingToken);
            }
        }
        catch (OperationCanceledException ex) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation(ex, "UpdateBookRatingService is stopping.");
        }
    }

    internal async Task RunRatingUpdate(CancellationToken stoppingToken)
    {
        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var repo = scope.ServiceProvider.GetRequiredService<IBookRepository>();
            await repo.UpdateRatingAsync(stoppingToken);
            logger.LogInformation("Books ratings updated.");
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update books ratings.");
        }
    }
}