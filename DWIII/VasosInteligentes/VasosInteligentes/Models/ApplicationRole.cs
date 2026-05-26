using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace VasosInteligentes.Models
{
    [CollectionName("Roles")]
    public class ApplicationRole:MongoDbIdentityRole
    {
    }
}
