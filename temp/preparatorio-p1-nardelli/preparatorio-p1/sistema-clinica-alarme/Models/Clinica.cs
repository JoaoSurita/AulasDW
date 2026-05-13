using MongoDB.Bson; // Necessário para lidar com tipos específicos do MongoDB
using MongoDB.Bson.Serialization.Attributes;// Atributos para mapeamento de propriedades

namespace sistema_clinica_alarme.Models
{
    // 2. Criação da(s) Model(s)
    public class Clinica
    {
        [BsonId] // Define esta propriedade como a chave primária no MongoDB
        [BsonRepresentation(BsonType.ObjectId)] // Permite que o Id seja tratado como string no C# mas ObjectId no banco
        public string? Id { get; set; } // Identificador único da clínica 

        [BsonElement("Nome")] // Mapeia o nome do campo no documento MongoDB
        public string ? Nome { get; set; } // Nome da clínica
        //public string Nome { get; set; } = string.Empty; // Nome da clínica

        [BsonElement("Alarme")] // Mapeia o status do alarme
        public bool Alarme { get; set; } = false; // Status do alarme: true (ligado) ou false (desligado) [cite: 6, 7]
    }
}
