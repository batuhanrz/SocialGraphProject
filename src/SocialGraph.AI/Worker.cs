namespace SocialGraph.AI;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SocialGraph AI Worker baslatildi.");

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Sentetik veri uretim iskeleti tetiklendi. Zaman: {time}", DateTimeOffset.Now);
            }

            // TODO (Sprint 2.5): Isra'nin sentetik veri ureteci cagrilacak.
            // Uretilen veriler HTTP Client ile API'ye yollanacak.

            // 15 saniyede bir tetiklenmesi saglandi (Sprint 2.4 plani geregi)
            await Task.Delay(15000, stoppingToken);
        }
    }
}
