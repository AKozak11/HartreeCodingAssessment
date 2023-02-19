namespace Common.Models
{
    public record RandomNumberData
    {
        public required string Key { get; set; }
        public RandomNumberValue Value { get; set; }
    }

    public record RandomNumberValue
    {
        public required DateTime Time { get; set; }
        public required float? Value { get; set; }
    }
}