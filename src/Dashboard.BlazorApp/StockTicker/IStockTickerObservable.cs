using Dashboard.BlazorApp.Models;

namespace Dashboard.BlazorApp.StockTicker;

public interface IStockTickerObservable
{
    public IObservable<StockModel> StreamStocks();
}
