namespace SistemaAcademico.Models
{
    public class Curso
    {
        public int CursoId { get; set; }
        public string? Nome { get; set; }
        public int Vagas { get; set; }
        // Coleção de Objetos do tipo Disciplina
        public ICollection<Disciplina>? Disciplinas { get; set; }
    }
}
