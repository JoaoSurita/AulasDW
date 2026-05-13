using MongoDB.Bson.Serialization.Attributes;

namespace P1_JoaoPedroSurita.Models
{
    public class Campeonatos
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? NomeDoEvento { get; set; }
        public DateTime DataEvento { get; set; }
        public string? Arena { get; set; }
        public int LimiteParticipantes { get; set; }
        public List<Robos> Robos { get; set; } = new List<Robos>();
    }
}
