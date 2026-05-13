using MongoDB.Bson.Serialization.Attributes;

namespace P1_JoaoPedroSurita.Models
{
    public class Robos
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? Nome { get; set; }
        public string? Categoria { get; set; }
        public int NivelBateria { get; set; }
        public string? Status { get; set; }
    }
}
