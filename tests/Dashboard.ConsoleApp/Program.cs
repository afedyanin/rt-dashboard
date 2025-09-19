using Dashboard.Application.Artemis;
using Microsoft.Extensions.Configuration;

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
            await marketDataConsumer.StartConsume(artemisSettings!.MarketDataTopic, Handle, cts.Token);

            /*
            Console.WriteLine("Starting trades consumer...");
            var tradesConsumer = new ArtemisConsumer(artemisSettings!);
            var t2 = tradesConsumer.StartConsume(artemisSettings!.TradesTopic, Handle, cts.Token);

            await Task.WhenAll(t1, t2);
            */
        }

        private static Task Handle(string message, IDictionary<string, string> props, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Message: {message}");

            if (props.Keys.Any())
            {
                foreach (var kvp in props)
                {
                    Console.WriteLine($"Prop: {kvp.Key}={kvp.Value}");
                }
            }

            return Task.CompletedTask;
        }
    }
}
