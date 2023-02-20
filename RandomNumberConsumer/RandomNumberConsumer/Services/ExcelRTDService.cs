using Common.Models;
using Common.Messaging;
using Confluent.Kafka;

namespace RandomNumberConsumer.Services
{
    public class ExcelRTDService : BackgroundService
    {
        private readonly ILogger<ExcelRTDService> _logger;
        private Func<string, IMessageReader<string, string>> _readerFactory;
        private IMessageReader<string, string> _messageReader;
        public ExcelRTDService(ILogger<ExcelRTDService> logger, Func<string, IMessageReader<string, string>> readerFactory)
        {
            _logger = logger;
            _readerFactory = readerFactory;
            // _messageReader = _readerFactory("excel");
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

            await Task.Run(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    // Message<string, string> message = _messageReader.ReadMessage();

                    // if (message is not null && !string.IsNullOrEmpty(message.Value))
                    // {
                    //     _logger.LogInformation($"Message received. Key = {message.Key}, Value = {message.Value}");
                    // }
                }
            });
        }
    }
}