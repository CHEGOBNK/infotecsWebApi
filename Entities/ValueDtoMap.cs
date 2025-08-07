using CsvHelper.Configuration;

namespace infotecsWebApi.Entities
{
    public sealed class ValueDtoMap : ClassMap<ValueDto>
    {
        public ValueDtoMap()
        {
            Map(m => m.Date).Index(0);
            Map(m => m.ExecutionTime).Index(1);
            Map(m => m.ValueV).Index(2);
        }
    }
}
