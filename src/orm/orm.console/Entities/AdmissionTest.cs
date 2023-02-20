namespace orm.console.Entities
{
    public class AdmissionTest : IEntity<int>
    {
        public int Id { get; set; }
        public int Course_id { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public double TestFees { get; set; }
    }
}