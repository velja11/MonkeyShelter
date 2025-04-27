using System.ComponentModel.DataAnnotations;

namespace MonkeyShelter.DTO
{
    public class MonkeyArrival
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public int SpeciesId { get; set; } 
        [Required]
        public double Weight { get; set; }
        [Required]
        public int ShelterId { get; set; }
    }
}
