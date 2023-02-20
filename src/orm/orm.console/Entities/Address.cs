namespace orm.console.Entities
{
    public class Address : IEntity<int>
    {
        public int Id { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }
}