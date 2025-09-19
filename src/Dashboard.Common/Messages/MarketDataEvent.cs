namespace Dashboard.Common.Messages
{
    public record class MarketDataEvent
    {
        public required string Ticker { get; set; }

        public decimal Price { get; set; }

        public DateTime TimeUtc { get; set; }

        public int Priority {  get; set; }
    }
}
