using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace VasosInteligentes.Models
{
    public class Planta
    {
        // Para setar o id no MongoDB (já que ele tem o uuId próprio), precisamos fazer algumas descritivas
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }

        public string? Nome { get; set; }

        [Display(Name = "Umidade Mínima")]
        public double UmidadeIdealMinima { get; set; }

        [Display(Name = "Umidade Máxima")]
        public double UmidadeIdealMaxima { get; set; }

        [Display(Name = "Luminosidade Ideal")]
        public double LuminosidadeIdeal { get; set; }
    }
}
