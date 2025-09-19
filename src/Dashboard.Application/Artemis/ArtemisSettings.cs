namespace Dashboard.Application.Artemis
{
    public class ArtemisSettings
    {
        public required string ConnectUriString { get; set; }
        
        public required string UserName { get; set; }
        
        public required string Password { get; set; }

        public required string MarketDataTopic { get; set; }

        public required string TradesTopic { get; set; }
    }
}
