using System.ComponentModel.DataAnnotations;

namespace SorteosAPI.Models
{
    public class User
    {
        public int IdUser { get; set; }
        public int IdClient { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }

    public class UserCreate
    {
        [Required(ErrorMessage = "El el Cliente es obligatorio.")]
        public int IdClient { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}