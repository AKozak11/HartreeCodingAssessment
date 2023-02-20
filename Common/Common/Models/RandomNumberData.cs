namespace Common.Models
{
    public record RandomNumberData
    {
        public string Key { get; set; }
        public RandomNumberValue Value { get; set; }
    }

    public record RandomNumberValue
    {
        public DateTime Time { get; set; }
        public float Value { get; set; }
    }
}