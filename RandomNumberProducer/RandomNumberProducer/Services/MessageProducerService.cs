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
            Random random = new Random((int)(DateTime.Now.Ticks % int.MaxValue));
            Stopwatch sw = new Stopwatch();

            await Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    Task delay = Task.Delay(1000, stoppingToken);
                    sw.Start();

                    foreach (string key in _messageConfig.Keys)
                    {
                        string message = BuildMessage(key, random);

                        await _messageWriter.WriteAsync(key, message);
                    }

                    await delay;

                    sw.Stop();
                    Console.WriteLine($"{_messageConfig.Keys.Length} message(s) Written in {Math.Round(sw.Elapsed.TotalSeconds)} second(s)");

                    sw.Reset();
                }
            });

        }

        private string BuildMessage(string key, Random random)
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