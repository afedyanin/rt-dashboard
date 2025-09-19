using Dashboard.Application.Artemis;
using Dashboard.Common.Messages;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Dashboard.ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var config = builder.Build();
            var artemisSettings = config.GetSection("ArtemisSettings").Get<ArtemisSettings>();
            var cts = new CancellationTokenSource();

            Console.WriteLine("Starting market data consumer...");
            var marketDataConsumer = new ArtemisConsumer(artemisSettings!);
            var t1 = marketDataConsumer.StartConsume(artemisSettings!.MarketDataTopic, Handle, cts.Token);

            Console.WriteLine("Starting trades consumer...");
            var tradesConsumer = new ArtemisConsumer(artemisSettings!);
            var t2 = tradesConsumer.StartConsume(artemisSettings!.TradesTopic, Handle, cts.Token);

            await Task.WhenAll(t1, t2);
        }

        private static Task Handle(string message, IDictionary<string, string> props, CancellationToken cancellationToken)
        {
            // Console.WriteLine($"Message: {message}");

            if (!props.TryGetValue("Type", out var messageType))
            {
                Console.WriteLine("Unknown message received!");
                return Task.CompletedTask;
            }

            if (messageType.Equals(typeof(MarketDataEvent).Name))
            {
                var marketData = JsonSerializer.Deserialize<MarketDataEvent>(message);
                Console.WriteLine($"Market Data: {marketData}");
                return Task.CompletedTask;
            }

            if (messageType.Equals(typeof(TradeEvent).Name))
            {
                var trade = JsonSerializer.Deserialize<TradeEvent>(message);
                Console.WriteLine($"Trade: {trade}");
                return Task.CompletedTask;
            }

            Console.WriteLine($"Unknown message type received: {messageType}");
            return Task.CompletedTask;
        }
    }
}
