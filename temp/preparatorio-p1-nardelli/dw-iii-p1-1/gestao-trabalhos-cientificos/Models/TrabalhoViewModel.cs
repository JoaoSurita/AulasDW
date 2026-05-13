namespace gestao_trabalhos_cientificos.Models
{
    public class TrabalhoViewModel
    {
        public string? Id { get; set; }// ID para os botões de ação
        public string? Titulo { get; set; }// Título do trabalho
        public string? AreaTematica { get; set; }// Área temática
        public int QuantidadeAutores { get; set; } // Total de autores na lista
        public double MediaNotas { get; set; } // Média calculada das avaliações
    }
}
