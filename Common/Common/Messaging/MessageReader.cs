using Confluent.Kafka;

namespace Common.Messaging
{
    public class MessageReader<TKey, TValue> : IMessageReader<TKey, TValue>
    {
        private ConsumerConfig _consumerConfig;
        private string _topic;
        private IConsumer<TKey, TValue> _consumer;
        /// <summary>
        /// Wrapper for kafka consumer. Consume messages from topic <paramref name="topic"/>
        /// using configurations specified in <paramref name="consumerConfig"/>
        /// </summary>
        public MessageReader(string topic, ConsumerConfig consumerConfig)
        {
            _topic = topic;
            _consumerConfig = consumerConfig;
            _consumer = new ConsumerBuilder<TKey, TValue>(_consumerConfig).Build();
            _consumer.Subscribe(_topic);
        }

        public Message<TKey, TValue> ReadMessage() => _consumer.Consume(100)?.Message;
        public ConsumeResult<TKey, TValue> Consume() => _consumer.Consume(100);
        public void Commit(ConsumeResult<TKey, TValue> consumeResult) => _consumer.Commit(consumeResult);

        public void Dispose()
        {
            _consumer.Close();
            _consumer.Dispose();
        }
    }
}