using Dashboard.BlazorApp.Models;
using System.Reactive.Subjects;

namespace Dashboard.BlazorApp.StockTicker;

public class StockTickerSubject : IStockTickerObservable, IStockTickerDataHandler, IDisposable
{
    private readonly Subject<StockModel> _stockSubject;

    public IObservable<StockModel> StreamStocks() => _stockSubject;

    public StockTickerSubject()
    {
        _stockSubject = new Subject<StockModel>();
    }

    public Task HandlePriceChange(StockModel stock)
    {
        _stockSubject.OnNext(stock);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _stockSubject?.Dispose();
    }
}
