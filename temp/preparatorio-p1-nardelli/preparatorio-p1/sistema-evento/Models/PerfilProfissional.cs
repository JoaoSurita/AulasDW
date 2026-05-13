namespace sistema_evento.Models
{
    public class PerfilProfissional
    {
        // Chave primária: identificador único do perfil 
        public int PerfilProfissionalId { get; set; }

        // Descrição do perfil (ex: Desenvolvedor, Estudante) 
        public string Descritivo { get; set; } = string.Empty;

        // Link opcional para o perfil no LinkedIn 
        public string? LinkedInUrl { get; set; }

        // Relacionamento 1:N - Um perfil pode pertencer a vários participantes 
        public virtual ICollection<Participante> Participantes { get; set; } = new List<Participante>();
    }
}
