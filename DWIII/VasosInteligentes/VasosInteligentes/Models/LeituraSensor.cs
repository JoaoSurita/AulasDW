using MongoDB.Bson.Serialization.Attributes;

namespace VasosInteligentes.Models
{
    public class LeituraSensor
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }
        public double Umidade { get; set; }
        public double Luminosidade { get; set; }
        public double Temperatura { get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string VasoId { get; set; }

        public DateTime DataLeitura { get; set; } = DateTime.UtcNow;
    }
}
