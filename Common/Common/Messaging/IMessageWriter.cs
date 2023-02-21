using Confluent.Kafka;

namespace Common.Messaging
{
    public interface IMessageWriter<TKey, TValue>
    {
        /// <summary>
        /// Produce a message to kafka topic specified in constructor.
        /// </summary>
        public Task<DeliveryResult<TKey, TValue>> WriteAsync(TKey key, TValue value);
    }
}