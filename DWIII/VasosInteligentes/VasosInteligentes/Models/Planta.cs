using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace VasosInteligentes.Models
{
    public class Planta
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? Nome { get; set; }
        [Display(Name = "Unidade Mínima")]
        public double UnidadeIdealMin { get; set; }
        [Display(Name = "Unidade Máxima")] 
        public double UnidadeIdealMax { get; set; }
        [Display(Name = "Luminosidade")] 
        public double LuminosidadeIdeal { get; set; }

    }
}
