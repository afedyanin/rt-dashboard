using Dashboard.Application.Jobs;
using Dashboard.Application.Mediator;
using Dashboard.BlazorApp.BackgroundServices;
using Dashboard.BlazorApp.Components;
using Dashboard.BlazorApp.Hubs;
using Dashboard.BlazorApp.StockTicker;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Text.Json.Serialization;

namespace Dashboard.BlazorApp
{
    public class Program
    {
        private const string _hangfireConnStringName = "HangfireConnection";

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services
                .AddControllers()
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddFluentUIComponents();

            builder.Services.AddHttpClient("backend", client => client.BaseAddress = new Uri("https://localhost:7086"));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Hubs
            builder.Services.AddHostedService<StockPricesUpdatingService>();
            builder.Services.AddSingleton<StockTickerSubject>();
            builder.Services.AddSingleton<IStockTickerObservable>(x => x.GetRequiredService<StockTickerSubject>());
            builder.Services.AddSingleton<IStockTickerDataHandler>(x => x.GetRequiredService<StockTickerSubject>());

            // Mediator
            builder.Services.AddMediator();

            // Hangfire
            var configuration = builder.Configuration;
            var hfConnectionString = configuration.GetConnectionString(_hangfireConnStringName);
            builder.Services.AddHangfire(hfConnectionString!);

            builder.Services.AddSignalR();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.UseAuthorization();

            app.MapControllers();

            app.MapHub<StocksHub>("/stocksHub");

            app.UseHangfireDashboard();

            app.Run();
        }
    }
}
