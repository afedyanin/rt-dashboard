using System.Text.Json;

namespace Dashboard.Common.Messages
{
    public static class DashboardMessageFactory
    {
        public static IDashboardMessage? Create(string message, string messageType)
        {
            if (messageType.Equals(typeof(MarketDataEvent).Name))
            {
                return JsonSerializer.Deserialize<MarketDataEvent>(message);
            }

            if (messageType.Equals(typeof(TradeEvent).Name))
            {
                return JsonSerializer.Deserialize<TradeEvent>(message);
            }

            return null;
        }
    }
}
