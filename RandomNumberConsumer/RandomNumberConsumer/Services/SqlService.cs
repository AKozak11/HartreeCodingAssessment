using Confluent.Kafka;
using Common.Messaging;

namespace RandomNumberConsumer.Services
{
    public class SqlService : BackgroundService
    {
        private readonly ILogger<SqlService> _logger;
        Func<string, IMessageReader<string, string>> _readerFactory;
        IMessageReader<string, string> _messageReader;
        public SqlService(ILogger<SqlService> logger, Func<string, IMessageReader<string, string>> readerFactory)
        {
            _logger = logger;
            _readerFactory = readerFactory;
            _messageReader = readerFactory("sql");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                Console.WriteLine("Cancel key clicked.");
                _messageReader.Dispose();
                cts.Cancel();
            };

            while (!stoppingToken.IsCancellationRequested)
            {
                Message<string, string> message = _messageReader.ReadMessage();

                if (message is not null) Console.WriteLine($"{message.Key} => {message.Value}");
                // _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}