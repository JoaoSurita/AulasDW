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
        [Display(Name = "Umidade Miníma")]
        
        public double UmidadeIdealMin { get; set; }
        [Display(Name = "Umidade Máxima")]
        public double UmidadeIdealMax { get; set; }
        [Display(Name = "Luminosidade Ideal")]
        public double LuminosidadeIdeal { get; set; }
    }
}
