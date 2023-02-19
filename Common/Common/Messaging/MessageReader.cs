using Confluent.Kafka;

namespace Common.Messaging
{
    public class MessageReader<TKey, TValue> : IMessageReader<TKey, TValue>
    {
        private ConsumerConfig _consumerConfig;
        private string _topic;
        private IConsumer<TKey, TValue> _consumer;
        public MessageReader(string topic, ConsumerConfig consumerConfig)
        {
            _topic = topic;
            _consumerConfig = consumerConfig;
            _consumer = new ConsumerBuilder<TKey, TValue>(_consumerConfig).Build();
            _consumer.Subscribe(_topic);
        }

        public Message<TKey, TValue> ReadMessage()
        {

            ConsumeResult<TKey, TValue> consumeResult = _consumer.Consume(500);

            if (consumeResult is null) return null;

            return new Message<TKey, TValue>
            {
                Key = consumeResult.Key,
                Value = consumeResult.Value
            };

        }

        public void Dispose()
        {
            _consumer.Close();
            _consumer.Dispose();
        }
    }
}