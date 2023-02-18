namespace Common.Models
{
    public class ConsumerMessageConfig : IMessageConfig
    {
        public string? KafkaTopic { get; set; }
        public string? ClientId { get; set; }
        public string? GroupId { get; set; }
    }
}