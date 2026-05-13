namespace sistema_evento.Models
{
    public class Evento
    {
        // Chave primária do evento 
        public int EventoId { get; set; }

        // Título ou nome do evento 
        public string Titulo { get; set; } = string.Empty;

        // Resumo ou descrição do que ocorrerá no evento 
        public string? Descricao { get; set; }

        // Data e hora da realização 
        public DateTime DataEvento { get; set; }

        // Chave Estrangeira para Tipo 
        public int TipoId { get; set; }
        // Navegação para o Tipo 
        public virtual Tipo? Tipo { get; set; }

        // Chave Estrangeira para Patrocinador 
        public int PatrocinadorId { get; set; }
        // Navegação para o Patrocinador 
        public virtual Patrocinador? Patrocinador { get; set; }

        // Relacionamento 1:N - Um evento pode ter muitas inscrições 
        public virtual ICollection<Inscricao> Inscricoes { get; set; } = new List<Inscricao>();
    }
}
