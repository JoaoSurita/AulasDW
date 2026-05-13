using Microsoft.EntityFrameworkCore;
using sistema_evento.Models;
namespace sistema_evento.Data
{
    public class AppDbContext : DbContext
    {
        // Construtor que passa as configurações de conexão para a classe base
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Representações das tabelas
        public DbSet<PerfilProfissional> PerfisProfissionais { get; set; }
        public DbSet<Participante> Participantes { get; set; }
        public DbSet<Tipo> Tipos { get; set; }
        public DbSet<Patrocinador> Patrocinadores { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Inscricao> Inscricoes { get; set; }

        public DbSet<Certificado> Certificados { get; set; }

        // Método para configurar regras específicas de modelagem (Fluent API)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configura a relação 1:1 entre Inscrição e Certificado
            modelBuilder.Entity<Inscricao>()
                .HasOne(i => i.Certificado) // Uma inscrição tem um certificado
                .WithOne(c => c.Inscricao)  // Um certificado pertence a uma inscrição
                .HasForeignKey<Certificado>(c => c.InscricaoId); // A FK fica na tabela Certificado

            base.OnModelCreating(modelBuilder); // Mantém as configurações base do EF
        }
    }
}
