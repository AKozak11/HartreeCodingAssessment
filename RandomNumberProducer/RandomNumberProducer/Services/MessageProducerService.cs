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

        private ProducerMessageConfig _producerMessageConfig;

        public MessageProducerService(ILogger<MessageProducerService> logger, IMessageWriter<string, string> messageWriter, ProducerMessageConfig producerMessageConfig)
        {
            _logger = logger;
            _messageWriter = messageWriter;
            _producerMessageConfig = producerMessageConfig;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Random random = new Random((int)(DateTime.Now.Ticks % int.MaxValue));
            Stopwatch sw = new Stopwatch();

            while (!stoppingToken.IsCancellationRequested)
            {
                Task delay = Task.Delay(1000, stoppingToken);
                sw.Start();

                foreach (string key in _producerMessageConfig.Keys)
                {
                    string message = GetMessage(key, random);

                    await _messageWriter.WriteAsync(key, message);

                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                await delay;

                sw.Stop();
                Console.WriteLine($"{_producerMessageConfig.Keys.Length} message(s) Written in {Math.Round(sw.Elapsed.TotalSeconds)} second(s)");

                sw.Reset();
            }
        }

        private string GetMessage(string key, Random random)
        {
            float randomNum = (float)random.NextDouble();

            RandomNumberData rnd = new RandomNumberData
            {
                Key = key,
                Value = new RandomNumberValue
                {
                    Time = DateTime.UtcNow,
                    Value = randomNum
                }
            };

            return JsonConvert.SerializeObject(rnd);
        }
    }
}