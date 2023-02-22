using Confluent.Kafka;
using Common.Messaging;
using Common.Models;
using System.Timers;

namespace RtdFunctions
{
    public class KeyedDataProvider : IDisposable
    {
        private string _key;
        public event Action<ConsumeResult<string, string>> NewData;
        public MessageReader<string, string> _messageReader;
        private System.Timers.Timer _timer;
        public KeyedDataProvider(string key)
        {
            _key = key;
            _messageReader = GetMessageReader(key);
            _timer = new System.Timers.Timer(100);
            _timer.Elapsed += (s, o) => NewData?.Invoke(Consume());
            _timer.Start();
        }
        private ConsumeResult<string, string> Consume()
        {
            ConsumeResult<string, string> consumeResult = _messageReader.Consume();
            
            if (consumeResult is not null && consumeResult.Message.Key == _key) return consumeResult;

            return null;
        }
        private MessageReader<string, string> GetMessageReader(string key)
        {
            ConsumerConfig consumerConfig = new ConsumerConfig
            {
                AutoOffsetReset = AutoOffsetReset.Earliest,
                BootstrapServers = "localhost:9092",
                EnableAutoCommit = false,
                SecurityProtocol = 0,
                SessionTimeoutMs = 6000,
                GroupId = $"{key}_excel_group",
                ClientId = $"{key}_excel"
            };
            return new MessageReader<string, string>("RANDOM_NUMBER_DATA", consumerConfig);
        }

        public void Dispose()
        {
            _messageReader.Dispose();
            _timer.Dispose();
        }
    }
}