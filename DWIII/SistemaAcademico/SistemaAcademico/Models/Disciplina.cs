using System.ComponentModel.DataAnnotations;

namespace SistemaAcademico.Models
{
    public class Disciplina
    {
        public int DisciplinaId { get; set; }
        public string? Nome { get; set; }
        public int Semestre { get; set; }
        // Mapeamento da Classe Curso
        [Display(Name = "Curso")]
        public int CursoId { get; set; }
        public Curso? Curso { get; set; }
    }
}
