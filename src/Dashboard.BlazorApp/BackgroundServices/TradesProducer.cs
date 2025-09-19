using Dashboard.Application.Artemis;
using Dashboard.Common.Messages;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Dashboard.BlazorApp.BackgroundServices
{
    public class TradesProducer : BackgroundService
    {
        private readonly ArtemisSettings _artemisSettings;

        public TradesProducer(IOptions<ArtemisSettings> options)
        {
            _artemisSettings = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var producer = new ArtemisProducer(_artemisSettings);

            var topic = _artemisSettings.TradesTopic;

            while (!stoppingToken.IsCancellationRequested)
            {
                var message = new TradeEvent
                {
                    Ticker = "SBER",
                    Price = 333.45m,
                    Qty = 10,
                    Priority = 7,
                    TimeUtc = DateTime.UtcNow,
                };

                var props = new Dictionary<string, string>()
                {
                    { "Type" , message.GetType().Name }
                };

                producer.Produce(topic, JsonSerializer.Serialize(message), props);

                await Task.Delay(5000);
            }
        }
    }
}
