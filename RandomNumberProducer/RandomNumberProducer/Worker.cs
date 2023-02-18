using Common.Messaging;

namespace RandomNumberProducer
{
    public class Worker : BackgroundService
    {
        private IMessageWriter<string, string> _messageWriter;
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger, IMessageWriter<string, string> messageWriter)
        {
            _logger = logger;
            _messageWriter = messageWriter;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Random r = new Random((int)(DateTime.Now.Ticks % 1000000));
            while (!stoppingToken.IsCancellationRequested)
            {
                long n = r.NextInt64(-1000000000, 1000000000);
                await _messageWriter.WriteAsync($"{n}", $"The random value is {n}");

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}