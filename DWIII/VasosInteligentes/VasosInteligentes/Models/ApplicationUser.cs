using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace VasosInteligentes.Models
{
    [CollectionName("Users")]
    public class ApplicationUser:MongoDbIdentityUser
    {
    }
}
