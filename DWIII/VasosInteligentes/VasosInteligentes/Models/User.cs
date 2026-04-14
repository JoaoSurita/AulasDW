using System.ComponentModel.DataAnnotations;
using Microsoft.Build.Tasks;

namespace VasosInteligentes.Models
{
    public class User
    {
        [Required]
        public string? Nome { get; set; }

        [Required]
        public string? Celular { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Email Inválido")]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
