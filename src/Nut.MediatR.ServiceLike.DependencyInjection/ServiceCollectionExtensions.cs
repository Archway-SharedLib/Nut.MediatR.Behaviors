using System;
using System.Linq;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Nut.MediatR.ServiceLike.DependencyInjection;

/// <summary>
/// <see cref="MediatR"/> を利用してメッセージを送信/発行するための機能を登録する <see cref="IServiceCollection"/> の拡張メソッドを定義します。
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// <see cref="MediatR"/> を利用してメッセージを送信/発行するための機能を登録します。
    /// </summary>
    /// <param name="services">元となる <see cref="IServiceCollection"/></param>
    /// <param name="assembly">メッセージの情報を探索する <see cref="Assembly"/></param>
    /// <param name="filterTypes">メッセージ送信の前後で実行されるフィルターの型</param>
    /// <returns>元となった <see cref="IServiceCollection" /></returns>
    public static IServiceCollection AddMediatRServiceLike(this IServiceCollection services, Assembly assembly, params Type[] filterTypes)
    {
        if (!(services.LastOrDefault(s => s.ServiceType == typeof(ServiceRegistry))?
            .ImplementationInstance is ServiceRegistry serviceRegistry))
        {
            serviceRegistry = new ServiceRegistry();
        }
        services.TryAddSingleton(serviceRegistry);

        var serviceDescriptions = assembly.GetTypes()
            .Where(MediatorServiceDescription.CanServicalize);
        foreach (var serviceDescription in serviceDescriptions)
        {
            serviceRegistry.Add(serviceDescription, true, filterTypes);
        }

        if (!(services.LastOrDefault(s => s.ServiceType == typeof(ListenerRegistry))?
            .ImplementationInstance is ListenerRegistry listenerRegistry))
        {
            listenerRegistry = new ListenerRegistry();
        }
        services.TryAddSingleton(listenerRegistry);

        var listenerDescriptions = assembly.GetTypes()
            .Where(MediatorListenerDescription.CanListenerize);
        foreach (var listenerDescription in listenerDescriptions)
        {
            listenerRegistry.Add(listenerDescription);
        }

        services.TryAddSingleton<IServiceLikeContextAccessor, ServiceLikeContextAccessor>();

        services.TryAddTransient(typeof(IMediatorClient), provider =>
        {
            var servRegistry = provider.GetService<ServiceRegistry>()!;
            var lisRegistry = provider.GetService<ListenerRegistry>()!;
            var serviceFactory = provider.GetService<ServiceFactory>()!;
            var scopedServiceFactoryFactory = new ScopedServiceFactoryFactory(provider.GetService<IServiceScopeFactory>()!);
            var serviceLikeLogger = provider.GetService<IServiceLikeLogger>()!;

            return new DefaultMediatorClient(
                servRegistry,
                lisRegistry,
                serviceFactory,
                scopedServiceFactoryFactory,
                serviceLikeLogger);
        });

        return services;
    }
}
