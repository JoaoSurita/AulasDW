using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace gestao_trabalhos_cientificos.Models
{
    // Classe principal do documento MongoDB
    public class Trabalho
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; } // Identificador único

        public string? Titulo { get; set; }// Título obrigatório
        public string? Resumo { get; set; }// Resumo obrigatório
        public string? AreaTematica { get; set; }// Área do trabalho
        public DateTime DataSubmissao { get; set; } = DateTime.Now; // Data de envio

        // Listas embutidas no mesmo documento 
        public List<Autor> Autores { get; set; } = new List<Autor>();
        public List<Avaliacao> Avaliacoes { get; set; } = new List<Avaliacao>();
    }
}
