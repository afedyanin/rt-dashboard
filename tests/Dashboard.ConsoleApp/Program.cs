using Dashboard.Application.Artemis;
using Dashboard.Application.MessageQueue;
using Dashboard.Common.Messages;
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
            var queue = new DashboardMessageQueue();

            Console.WriteLine("Starting market data consumer...");
            var marketDataConsumer = new ArtemisDashboardConsumer(queue, artemisSettings!);
            var t1 = marketDataConsumer.StartConsume(artemisSettings!.MarketDataTopic, cts.Token);

            Console.WriteLine("Starting trades consumer...");
            var tradesConsumer = new ArtemisDashboardConsumer(queue, artemisSettings!);
            var t2 = tradesConsumer.StartConsume(artemisSettings!.TradesTopic, cts.Token);

            var t3 = StartProcessingMessageQueue(queue, cts.Token);

            await Task.WhenAll(t1, t2, t3);
        }

        private static async Task StartProcessingMessageQueue(IMessageQueueConsumer messageQueue, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!messageQueue.TryGet(out var message))
                {
                    Console.WriteLine("--------------------------");
                    await Task.Delay(5000, cancellationToken);
                    continue;
                }

                Console.WriteLine($"{message}");
            }
        }
    }
}
