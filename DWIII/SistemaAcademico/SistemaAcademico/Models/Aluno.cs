using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SistemaAcademico.Models
{
    public class Aluno
    {
        public int AlunoId { get; set; }
        // Para aparecer ao usuário 
        [Display(Name = "RA")]
        // Caso o usuário não preencher o campo
        [Required(ErrorMessage = "O RA é Obrigatório.")]
        // Para informar ao usuário o tamanho máximo e mínimo do RA
        [StringLength(10, MinimumLength = 4, ErrorMessage = "O RA deve ter entre 4 a 10 caracteres.")]
        public string? Ra { get; set; }
        // Mapeamento da Classe Usuario
        public int UsuarioId{ get; set; }
        public Usuario? Usuario { get; set; }

    }
}
