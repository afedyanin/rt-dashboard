namespace Dashboard.Common.Messages
{
    public record class MarketDataEvent : IDashboardMessage
    {
        public int Priority { get; set; }

        public required string Ticker { get; set; }

        public decimal Price { get; set; }

        public DateTime TimeUtc { get; set; }
    }
}
