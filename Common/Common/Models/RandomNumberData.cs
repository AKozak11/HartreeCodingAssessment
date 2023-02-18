namespace Common.Models
{
    public class RandomNumberData
    {
        public required string Key { get; set; }
        public RandomNumberValue Value { get; set; }
    }

    public class RandomNumberValue
    {
        public required DateTime Time { get; set; }
        public required float? Value { get; set; }
    }
}