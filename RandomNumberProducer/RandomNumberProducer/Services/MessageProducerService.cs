using Confluent.Kafka;
using Common.Messaging;
using Common.Models;
using Newtonsoft.Json;
using System.Diagnostics;

namespace RandomNumberProducer.Services
{
    public class MessageProducerService : BackgroundService
    {
        private readonly ILogger<MessageProducerService> _logger;
        private IMessageWriter<string, string> _messageWriter;

        private MessageConfig _messageConfig;

        public MessageProducerService(ILogger<MessageProducerService> logger, IMessageWriter<string, string> messageWriter, MessageConfig messageConfig)
        {
            _logger = logger;
            _messageWriter = messageWriter;
            _messageConfig = messageConfig;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                _logger.LogCritical($"Cancel key clicked. Application shut down at {DateTime.UtcNow.ToString()}");
                cts.Cancel();
            };

            Random random = new Random((int)(DateTime.Now.Ticks % int.MaxValue));
            Stopwatch sw = new Stopwatch();

            await Task.Run(async () =>
            {

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        Task delay = Task.Delay(1000, stoppingToken);
                        sw.Start();

                        foreach (string key in _messageConfig.Keys)
                        {
                            string message = BuildMessage(key, random);

                            _logger.LogInformation($"Writing message to Kafka. key = {key}");

                            await _messageWriter.WriteAsync(key, message);

                            _logger.LogInformation($"Message written to Kafka. key = {key}");
                        }

                        await delay;

                        sw.Stop();
                        _logger.LogInformation($"{_messageConfig.Keys.Length} message(s) Written in {Math.Round(sw.Elapsed.TotalSeconds)} second(s)");
                        sw.Reset();

                    }
                    catch (ProduceException<string, string> pEx)
                    {
                        _logger.LogError($"There was an issue writing to Kafka. Exception: {pEx}");
                    }
                    catch (KafkaException ke)
                    {
                        _logger.LogError($"Kafka Error. Exception {ke}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Unhandled error while processing. Exception: {ex}");
                    }
                }
            });

        }

        private string BuildMessage(string key, Random random)
        {
            _logger.LogInformation($"Generating random decimal number.");
            
            float randomNum = (float)random.NextDouble();
            
            _logger.LogInformation($"Decimal number generated: {randomNum}");

            RandomNumberData rnd = new RandomNumberData
            {
                Key = key,
                Value = new RandomNumberValue
                {
                    Time = DateTime.UtcNow,
                    Value = randomNum
                }
            };

            _logger.LogInformation($"Serializing message data.");

            return JsonConvert.SerializeObject(rnd);
        }
    }
}