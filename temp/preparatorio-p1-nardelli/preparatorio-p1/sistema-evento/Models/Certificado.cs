namespace sistema_evento.Models
{
    public class Certificado
    {
        // Chave primária do certificado 
        public int CertificadoId { get; set; }

        // Data de emissão do documento 
        public DateTime DataEmissao { get; set; }

        // Link para download ou visualização do certificado 
        public string Link { get; set; } = string.Empty;

        // Chave Estrangeira para Inscrição (Relacionamento 1:1) 
        public int InscricaoId { get; set; }
        // Navegação reversa para a inscrição 
        public virtual Inscricao? Inscricao { get; set; }
    }
}
