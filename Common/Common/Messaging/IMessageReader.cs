using Confluent.Kafka;

namespace Common.Messaging
{
    public interface IMessageReader<TKey, TValue>
    {
        public Message<TKey, TValue> ReadMessage();
        public void Dispose();
    }
}