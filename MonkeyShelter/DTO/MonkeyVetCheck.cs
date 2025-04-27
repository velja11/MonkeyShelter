namespace MonkeyShelter.DTO
{
    public class MonkeyVetCheck
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int SpeciesId { get; set; }
        public double Weight { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime? DepartureDate { get; set; }
        public int ShelterId { get; set; }
        public bool NeedsVetCheck { get; set; }
    }
}
