using Confluent.Kafka;

namespace Common.Messaging
{
    public interface IMessageReader<TKey, TValue>
    {
        /// <summary>
        /// Read a message from the kafka topic specified in the constructor.
        /// By default will block for 100 milliseconds or until a kafka consumeresult is available.
        /// </summary>
        public Message<TKey, TValue> ReadMessage();
        public void Dispose();
    }
}