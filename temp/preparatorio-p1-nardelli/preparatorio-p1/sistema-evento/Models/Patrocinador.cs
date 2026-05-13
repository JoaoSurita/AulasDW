using System.Diagnostics;

namespace sistema_evento.Models
{
    public class Patrocinador
    {
        // Chave primária do patrocinador 
        public int PatrocinadorId { get; set; }

        // Nome da empresa patrocinadora 
        public string NomeEmpresa { get; set; } = string.Empty;

        // Site oficial da empresa 
        public string? Website { get; set; }

        // Relacionamento 1:N - Um patrocinador pode apoiar vários eventos 
        public virtual ICollection<Evento> Eventos { get; set; } = new List<Evento>();
    }
}
