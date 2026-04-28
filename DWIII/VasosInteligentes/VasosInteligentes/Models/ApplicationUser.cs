using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace VasosInteligentes.Models
{
    [CollectionName("users")]
    public class ApplicationUser:MongoDbIdentityUser
    {
    }
}
