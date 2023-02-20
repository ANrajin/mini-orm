namespace orm.console.Entities
{
    public class Topic : IEntity<int>
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public int Course_id { get; set; }
        public string? Description { get; set; }
        public List<Session>? Sessions { get; set; }
    }
}