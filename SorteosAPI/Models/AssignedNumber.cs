using System.ComponentModel.DataAnnotations;

namespace SorteosAPI.Models
{
    public class AssignedNumberRaffer
    {
        [Required(ErrorMessage = "El cliente es obligatorio.")]
        public int IdClient { get; set; }

        [Required(ErrorMessage = "El sorteo es obligatorio.")]
        public int IdRaffle { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio.")]
        public int IdUser { get; set; }

        public string Number { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class ListNumberRaffer
    {
        public int IdAssignedNumber { get; set; }
        public int IdClient { get; set; }
        public string ClientName { get; set; }
        public int IdRaffleByClient { get; set; }
        public string RaffleName { get; set; }
        public int IdUser { get; set; }
        public string UserName { get; set; }
        public string Number { get; set; }
        public bool IsActive { get; set; } = true;
    }
}