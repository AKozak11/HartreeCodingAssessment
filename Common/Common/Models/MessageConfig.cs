namespace Common.Models
{
    public class MessageConfig : IMessageConfig
    {
        public string? KafkaTopic { get; set; }
        public string[]? Keys { get; set; }
    }
}