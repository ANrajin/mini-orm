namespace orm.console.Entities
{
    public class Phone : IEntity<int>
    {
        public int Id { get; set; }
        public string? Number { get; set; }
        public string? Extension { get; set; }
        public string? CountryCode { get; set; }
    }
}