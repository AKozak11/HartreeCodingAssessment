namespace RandomNumberConsumer.Services
{

    public class ExcelRTDService : BackgroundService
    {
        private readonly ILogger<ExcelRTDService> _logger;

        public ExcelRTDService(ILogger<ExcelRTDService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}