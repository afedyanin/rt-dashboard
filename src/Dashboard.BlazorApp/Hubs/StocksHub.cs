using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;
using Dashboard.BlazorApp.Models;
using Dashboard.BlazorApp.StockTicker;
using Dashboard.BlazorApp.Extensions;

namespace Dashboard.BlazorApp.Hubs;

public class StocksHub : Hub
{
    private readonly IStockTickerObservable _stockTickerObservable;
    private readonly IStockTickerDataHandler _stockTickerDataHandler;

    private readonly Dictionary<string, StockModel> _stocks;

    public StocksHub(
        IStockTickerObservable stockTickerObservable,
        IStockTickerDataHandler stockTickerDataHandler)
    {
        _stockTickerObservable = stockTickerObservable;
        _stocks = InitStocks();
        _stockTickerDataHandler = stockTickerDataHandler;
    }

    public Task<StockModel[]> GetSnapshot()
    {
        return Task.FromResult(_stocks.Values.ToArray());
    }

    public ChannelReader<StockModel> StreamStocks()
    {
        return _stockTickerObservable.StreamStocks().AsChannelReader(10);
    }

    public Task OnNewLastChangeInput(StockModel stockModel)
    {
        stockModel.UpdatedAt = DateTime.UtcNow;
        stockModel.DayOpen = stockModel.LastChange;
        _stocks[stockModel.Symbol] = stockModel;

        _stockTickerDataHandler.HandlePriceChange(_stocks[stockModel.Symbol]);

        return Task.CompletedTask;
    }

    public override Task OnConnectedAsync()
    {
        Console.WriteLine("Connected!");
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"Disconnected! Ex={exception?.Message}");
        return base.OnDisconnectedAsync(exception);
    }

    private Dictionary<string, StockModel> InitStocks()
    {
        var res = new Dictionary<string, StockModel>();

        res.Add("AAA", new StockModel
        {
            Symbol = "AAA",
            DayOpen = 100,
            DayLow = 90,
            DayHigh = 120,
            LastChange = 103,
            Change = 3,
            PercentChange = 0.04,
            UpdatedAt = DateTime.UtcNow,
        });

        res.Add("BBB", new StockModel
        {
            Symbol = "BBB",
            DayOpen = 100,
            DayLow = 90,
            DayHigh = 120,
            LastChange = 103,
            Change = 3,
            PercentChange = 0.04,
            UpdatedAt = DateTime.UtcNow,
        });

        res.Add("CCC", new StockModel
        {
            Symbol = "CCC",
            DayOpen = 100,
            DayLow = 90,
            DayHigh = 120,
            LastChange = 103,
            Change = 3,
            PercentChange = 0.04,
            UpdatedAt = DateTime.UtcNow,
        });

        return res;
    }
}
