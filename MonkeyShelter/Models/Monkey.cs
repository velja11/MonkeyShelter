namespace MonkeyShelter.Models
{
    public class Monkey
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int SpeciesId { get; set; }
        public double Weight { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime? DepartureDate { get; set; }
        public int ShelterId { get; set; }
    }
}
