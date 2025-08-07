namespace infotecsWebApi.Entities
{
    public class ResultFilterDto
    {
        public string? FileName { get; set; }
        public float? MinAverageValue { get; set; }
        public float? MaxAverageValue { get; set; }
        public DateTimeOffset? MinMinimalDate { get; set; }
        public DateTimeOffset? MaxMinimalDate { get; set; }
        public TimeSpan? MinAverageExecutionTime { get; set; }
        public TimeSpan? MaxAverageExecutionTime { get; set; }

        public bool HasAnyFilter()
        {
            return !string.IsNullOrEmpty(FileName)
                || MinAverageValue.HasValue
                || MaxAverageValue.HasValue
                || MinMinimalDate.HasValue
                || MaxMinimalDate.HasValue
                || MinAverageExecutionTime.HasValue
                || MaxAverageExecutionTime.HasValue;
        }

    }


}
