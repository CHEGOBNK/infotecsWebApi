namespace infotecsWebApi.Entities
{
    public class Result
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public TimeSpan DeltaDate { get; set; }
        public DateTimeOffset MinimalDate { get; set; }
        public TimeSpan AverageExecutionTime { get; set; }
        public float AverageValue { get; set; }
        public float MedianValue { get; set; }
        public float MaximalValue { get; set; }
        public float MinimalValue { get; set; }

    }
}
