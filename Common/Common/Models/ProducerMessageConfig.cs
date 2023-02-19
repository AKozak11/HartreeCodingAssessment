namespace Common.Models
{
    public class ProducerMessageConfig : IMessageConfig
    {
        public string? KafkaTopic { get; set; }
        public string[]? Keys { get; set; }
    }
}