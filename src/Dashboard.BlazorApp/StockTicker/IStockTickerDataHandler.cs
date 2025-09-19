using Dashboard.BlazorApp.Models;

namespace Dashboard.BlazorApp.StockTicker;

public interface IStockTickerDataHandler
{
    public Task HandlePriceChange(StockModel stock);
}
