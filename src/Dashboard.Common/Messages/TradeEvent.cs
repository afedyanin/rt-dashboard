namespace Dashboard.Common.Messages
{
    public record class TradeEvent : IDashboardMessage
    {
        public int Priority { get; set; }

        public required string Ticker { get; set; }

        public decimal Price { get; set; }

        public decimal Qty { get; set; }

        public DateTime TimeUtc { get; set; }
    }
}
