namespace sistema_evento.Models
{
    public class Inscricao
    {
        // Chave primária da inscrição 
        public int InscricaoId { get; set; }

        // Data em que a inscrição foi realizada 
        public DateTime DataInscricao { get; set; }

        // Chave Estrangeira para Evento 
        public int EventoId { get; set; }
        // Navegação para o Evento 
        public virtual Evento? Evento { get; set; }

        // Chave Estrangeira para Participante 
        public int ParticipanteId { get; set; }
        // Navegação para o Participante 
        public virtual Participante? Participante { get; set; }

        // Relacionamento 1:1 - Uma inscrição gera um certificado 
        public virtual Certificado? Certificado { get; set; }
    }
}
