using Confluent.Kafka;

namespace Common.Messaging
{
    public class MessageWriter<TKey, TValue> : IMessageWriter<TKey, TValue>
    {
        private ProducerConfig _producerConfig;
        private string _topic;
        /// <summary>
        /// Wrapper for kafka producer. Produce messages to topic <paramref name="topic"/>
        /// using configurations specified in <paramref name="producerConfig"/>
        /// </summary>
        public MessageWriter(string topic, ProducerConfig producerConfig)
        {
            _topic = topic;
            _producerConfig = producerConfig;
        }

        public async Task<DeliveryResult<TKey, TValue>> WriteAsync(TKey key, TValue value)
        {
            using (IProducer<TKey, TValue> producer = new ProducerBuilder<TKey, TValue>(_producerConfig).Build())
            {

                Message<TKey, TValue> kafkaMessage = new Message<TKey, TValue>
                {
                    Key = key,
                    Value = value
                };

                DeliveryResult<TKey, TValue> result = await producer.ProduceAsync(_topic, kafkaMessage);

                return result;
            }
        }
    }
}