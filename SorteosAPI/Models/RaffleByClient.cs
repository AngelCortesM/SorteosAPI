using System.ComponentModel.DataAnnotations;

namespace SorteosAPI.Models
{
    public class RaffleByClient
    {
        public int IdRaffleByClient { get; set; }
        public int IdClient { get; set; }
        public int IdRaffle { get; set; }
        public string ClientName { get; set; }
        public string RaffleName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class RaffleByClientAssign
    {
        [Required(ErrorMessage = "El cliente es obligatorio.")]
        public int IdClient { get; set; }

        [Required(ErrorMessage = "El sorteo es obligatorio.")]
        public int IdRaffle { get; set; }

        public bool IsActive { get; set; }
    }
}