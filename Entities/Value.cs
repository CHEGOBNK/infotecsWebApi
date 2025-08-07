namespace infotecsWebApi.Entities
{
    public class Value
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public DateTimeOffset Date { get; set; }
        public TimeSpan ExecutionTime { get; set; }
        public float ValueV { get; set; }
    }
}
