
using Dashboard.Application.Artemis;
using Dashboard.Common.Messages;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Dashboard.BlazorApp.BackgroundServices
{
    public class MarketDataProducer : BackgroundService
    {
        private readonly ArtemisSettings _artemisSettings;

        public MarketDataProducer(IOptions<ArtemisSettings> options)
        {
            _artemisSettings = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var producer = new ArtemisProducer(_artemisSettings);

            var topic = _artemisSettings.MarketDataTopic;

            while (!stoppingToken.IsCancellationRequested)
            { 
                var message = new MarketDataEvent 
                { 
                    Ticker = "SBER",
                    Price = 300.45m,
                    Priority = 10,
                    TimeUtc = DateTime.UtcNow,
                };

                var props = new Dictionary<string, string>()
                {
                    { "Type" , message.GetType().Name }
                };

                producer.Produce(topic, JsonSerializer.Serialize(message));

                await Task.Delay(3000);
            }
        }
    }
}
