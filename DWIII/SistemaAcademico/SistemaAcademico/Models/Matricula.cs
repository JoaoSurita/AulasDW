namespace SistemaAcademico.Models
{
    public class Matricula
    {
        public int MatriculaId { get; set; }
        public DateTime DataMatricula { get; set; }
        public int AlunoId { get; set; }
        public Aluno? Aluno { get; set; }
        public int CursoId { get; set; }
        public Curso? Curso { get; set; }
    }
}
