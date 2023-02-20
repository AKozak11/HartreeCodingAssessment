using Confluent.Kafka;
using Common.Messaging;
using Common.Models;
using Newtonsoft.Json;
using EntityConnector.Models;
using Microsoft.EntityFrameworkCore;

namespace RandomNumberConsumer.Services
{
    public class SqlService : BackgroundService
    {
        private readonly ILogger<SqlService> _logger;
        Func<string, IMessageReader<string, string>> _readerFactory;
        IMessageReader<string, string> _messageReader;
        DbContextOptions<Context> _contextOptions;
        public SqlService(ILogger<SqlService> logger, Func<string, IMessageReader<string, string>> readerFactory, DbContextOptions<Context> contextOptions)
        {
            _logger = logger;
            _readerFactory = readerFactory;
            _messageReader = _readerFactory("sql");
            _contextOptions = contextOptions;
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
                    Message<string, string> message = _messageReader.ReadMessage();

                    if (message is not null && !string.IsNullOrEmpty(message.Value))
                    {

                        _logger.LogInformation($"Message received. Key = {message.Key}, Value = {message.Value}");

                        RandomNumberData rnd = JsonConvert.DeserializeObject<RandomNumberData>(message.Value);
                        using (Context context = new Context(_contextOptions))
                        {

                            NumberData dbEntry = new NumberData
                            {
                                Key = message.Key,
                                Time = rnd.Value.Time,
                                Value = rnd.Value.Value
                            };

                            context.Data.Add(dbEntry);
                            context.SaveChanges();
                        }
                    }
                }
            });


        }
    }
}