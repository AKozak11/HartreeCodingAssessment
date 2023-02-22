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
        IMessageReader<string, string> _messageReader;
        DbContextOptions<Context> _contextOptions;
        public SqlService(ILogger<SqlService> logger, IMessageReader<string, string> messageReader, DbContextOptions<Context> contextOptions)
        {
            _logger = logger;
            _messageReader = messageReader;
            _contextOptions = contextOptions;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                _logger.LogCritical($"Cancel key clicked. Application shut down at {DateTime.UtcNow.ToString()}");
                cts.Cancel();
            };

            await Task.Run(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        Message<string, string> message = _messageReader.ReadMessage();

                        if (message is not null && !string.IsNullOrEmpty(message.Value))
                        {
                            _logger.LogInformation($"Message received. key = {message.Key}, value = {message.Value}");

                            RandomNumberData rnd = JsonConvert.DeserializeObject<RandomNumberData>(message.Value);
                            using (Context context = new Context(_contextOptions))
                            {
                                NumberData dbEntry = new NumberData
                                {
                                    Key = message.Key,
                                    Time = rnd.Value.Time,
                                    Value = rnd.Value.Value
                                };

                                _logger.LogInformation($"Saving message to database. key = {message.Key}");

                                context.Data.Add(dbEntry);
                                context.SaveChanges();

                                _logger.LogInformation($"Message successfully saved to database. key = {message.Key}");
                            }
                        }
                    }
                    catch (ConsumeException cEx)
                    {
                        _logger.LogError($"There was an issue reading from Kafka. Exception: {cEx}");
                    }
                    catch (Microsoft.Data.SqlClient.SqlException sqlEx)
                    {
                        _logger.LogError($"Encoutered an error while writing to the database. Exception: {sqlEx}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Unhandled error while processing. Exception: {ex}");
                    }
                }
            });
        }
    }
}