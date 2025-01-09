using System.ComponentModel.DataAnnotations;

namespace SorteosAPI.Models
{
    public class Raffle
    {
        public int IdRaffle { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; }
    }

    public class RaffleCreate
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Name { get; set; }

        public bool IsActive { get; set; }
    }
}