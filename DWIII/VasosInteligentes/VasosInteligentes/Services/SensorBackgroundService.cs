using MongoDB.Bson;
using MongoDB.Driver;
using VasosInteligentes.Data;
using VasosInteligentes.Models;

namespace VasosInteligentes.Services
{
    public class SensorBackgroundService : BackgroundService
    {
        private readonly ContextMongoDb _context;
        private readonly Random _random = new Random();

        public SensorBackgroundService(ContextMongoDb context)
        {
            _context = context;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var vasos = await _context.Vaso.Find(new BsonDocument())
                        .ToListAsync(stoppingToken);
                    foreach(var vaso in vasos)
                    {
                        if (string.IsNullOrEmpty(vaso.Id)) continue;
                        var novaLeitura = new LeituraSensor
                        {
                            VasoId = vaso.Id,
                            Temperatura = Math.Round(_random.NextDouble() * (32 - 18) + 18, 1),
                            Umidade = Math.Round(_random.NextDouble() * (80 - 20) + 20, 1),
                            Luminosidade = Math.Round(_random.NextDouble() * (100 - 10) + 10, 1),
                            DataLeitura = DateTime.UtcNow

                        };
   await _context.LeituraSensor.InsertOneAsync(novaLeitura, null, stoppingToken);
                    }

                }
                catch(Exception ex)
                {
                   Console.WriteLine($"Problema na simulação das leituras dos sensores" +
                             $" :{ex.Message}");
                }
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
