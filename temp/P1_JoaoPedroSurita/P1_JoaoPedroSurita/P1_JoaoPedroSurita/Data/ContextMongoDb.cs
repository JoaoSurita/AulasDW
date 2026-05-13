using Microsoft.Extensions.Options;
using MongoDB.Driver;
using P1_JoaoPedroSurita.Models;

namespace P1_JoaoPedroSurita.Data
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
                    EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12
                };
            }
            var client = new MongoClient(clientSettings);
            _database = client.GetDatabase(mongoSettings.Database);
        }

        public IMongoCollection<Robos> Robos
        {
            get { return _database.GetCollection<Robos>("Robos"); }
        }
        public IMongoCollection<Campeonatos> Campeonatos
        {
            get { return _database.GetCollection<Campeonatos>("Campeonatos"); }
        }
    }
}
