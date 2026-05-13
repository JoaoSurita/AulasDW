namespace sistema_evento.Models
{
    public class Participante
    {
       // Chave primária do participante 
        public int ParticipanteId { get; set; }

       // Nome completo do participante 
        public string Nome { get; set; } = string.Empty;

       // E-mail para contato e login 
        public string Email { get; set; } = string.Empty;

       // Número de celular/telefone 
        public string Celular { get; set; } = string.Empty;

       // Senha de acesso (máximo 8 caracteres conforme o diagrama) 
        public string Senha { get; set; } = string.Empty;

       // Chave Estrangeira para PerfilProfissional 
        public int PerfilProfissionalId { get; set; }
       // Propriedade de navegação para acessar os dados do perfil vinculado 
        public virtual PerfilProfissional? PerfilProfissional { get; set; }

       // Relacionamento 1:N - Um participante pode ter várias inscrições 
        public virtual ICollection<Inscricao> Inscricoes { get; set; } = new List<Inscricao>();
    }
}
