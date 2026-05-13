namespace gestao_trabalhos_cientificos.Models
{
    // Classe para representar as avaliações dentro do trabalho
    public class Avaliacao
    {
        public int Nota { get; set; } // Nota de 1 a 5
        public string? Comentario { get; set; } // Comentário da avaliação
        //public string Comentario { get; set; } = string.Empty; // Comentário da avaliação
        public DateTime DataAvaliacao { get; set; } = DateTime.Now; // Data automática da avaliação
    }
}
