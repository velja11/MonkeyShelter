using System.ComponentModel.DataAnnotations;

namespace MonkeyShelter.DTO
{
    public class UpdateWeightDTO
    {
        [Range(0.1, 500, ErrorMessage = "Weight must be greater than 0.")]
        public double Weight { get; set; }
    }
}
