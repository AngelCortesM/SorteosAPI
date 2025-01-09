using System.ComponentModel.DataAnnotations;

namespace SorteosAPI.Models
{
    public class Client
    {
        public int? IdClient { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Name { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class ClientCreate
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Name { get; set; }

        public bool IsActive { get; set; }
    }
}