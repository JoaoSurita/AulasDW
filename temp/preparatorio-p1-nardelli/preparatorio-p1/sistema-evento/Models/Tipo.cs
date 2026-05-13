using System.Diagnostics;

namespace sistema_evento.Models
{
    public class Tipo
    {
        // Chave primária do tipo 
        public int TipoId { get; set; }

        // Descrição do tipo de evento 
        public string Descritivo { get; set; } = string.Empty;

        // Relacionamento 1:N - Um tipo pode ser aplicado a vários eventos 
        public virtual ICollection<Evento> Eventos { get; set; } = new List<Evento>();
    }
}
