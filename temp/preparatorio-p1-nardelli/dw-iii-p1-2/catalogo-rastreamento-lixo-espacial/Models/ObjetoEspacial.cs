using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace catalogo_rastreamento_lixo_espacial.Models
{
    public class ObjetoEspacial
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
       
        public string? Nome { get; set; }
        public int NoradId { get; set; }
       
        public string? TipoObjeto { get; set; }

        // Lista embutida conforme solicitado
        public List<HistoricoOrbital> HistoricoOrbital { get; set; } = new List<HistoricoOrbital>();
    }
}
