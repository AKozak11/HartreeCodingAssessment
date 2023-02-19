namespace Common.Models
{
    public class ConsumerMessageConfig : IMessageConfig
    {
        public string? KafkaTopic { get; set; }
    }
}