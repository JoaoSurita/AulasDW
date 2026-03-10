using System.Security.Authentication;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using VasosInteligentes.Models;

namespace VasosInteligentes.Data
{
    public class ContextMongoDb
    {
        private readonly IMongoDatabase _database;
        public ContextMongoDb(IOptions<MongoSettings> settings)
        {
            var mongoSettings = settings.Value;
            var mongoUrl = new MongoUrl(mongoSettings.ConnectionString);
            var clientSettings = MongoClientSettings.FromUrl(mongoUrl);
            if (mongoSettings.IsSsl)
            {
                clientSettings.SslSettings = new SslSettings
                {
                    EnabledSslProtocols = SslProtocols.Tls12
                };
            }
            var client = new MongoClient(clientSettings);
            _database = client.GetDatabase(mongoSettings.Database);
        }
        public IMongoCollection<Vaso> Vaso
        {
            get { return _database.GetCollection<Vaso>("Vaso"); }
        }public IMongoCollection<Planta> Planta
        {
            get { return _database.GetCollection<Planta>("Planta"); }
        }
    }
}
