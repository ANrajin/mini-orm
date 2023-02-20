namespace orm.console.Entities
{
    public class Session : IEntity<int>
    {
        public int Id { get; set; }
        public int Topic_id { get; set; }
        public int DurationInHour { get; set; }
        public string? LearningObjective { get; set; }
    }
}