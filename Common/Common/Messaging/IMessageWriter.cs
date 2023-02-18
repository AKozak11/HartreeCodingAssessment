using Confluent.Kafka;

namespace Common.Messaging
{
    public interface IMessageWriter<TKey, TValue>
    {
        public Task<DeliveryResult<TKey, TValue>> WriteAsync(TKey key, TValue value);
    }
}