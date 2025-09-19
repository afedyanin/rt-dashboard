using System.Reflection;
using Dashboard.Common.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace Dashboard.Application.Mediator;

public static class MediatorRegistrar
{
    public static IServiceCollection AddMediator(this IServiceCollection services)
    {
        services.AddSingleton<IMediator, Mediatr>();
        return services;
    }

    public static IServiceCollection AddMediatorHandlers(this IServiceCollection services, Assembly assembly)
    {
        services.Scan(scan =>
            scan.FromAssemblies(assembly)
            .AddClasses(classes =>
                classes.AssignableTo(typeof(IRequestHandler<>)), publicOnly: false)
                .AsImplementedInterfaces()
            .AddClasses(classes =>
                classes.AssignableTo(typeof(IRequestHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
        );

        return services;
    }
}
