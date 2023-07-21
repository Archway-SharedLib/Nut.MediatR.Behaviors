using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Nut.MediatR;

internal static class ServiceProviderExtensions
{
    public static IEnumerable<T> GetServicesOrEmpty<T>(this IServiceProvider provider)
    {
        var enumerableType = typeof(IEnumerable<>).MakeGenericType(typeof(T));
        return GetServices(provider, enumerableType) as IEnumerable<T> ?? Enumerable.Empty<T>();
    }

    public static T? GetFirstServiceOrDefault<T>(this IServiceProvider provider)
    {
        return GetFirstServiceOrDefault<T>(provider, default);
    }

    private static T? GetFirstServiceOrDefault<T>(this IServiceProvider provider, T? defaultValue)
    {
        var instances = provider.GetServicesOrEmpty<T>();
        if (!instances.Any()) return defaultValue;
        return instances.First();
    }

    private static object GetServices(IServiceProvider provider, Type serviceType)
    {
        if (provider == null)
        {
            throw new ArgumentNullException(nameof(provider));
        }

        if (serviceType == null)
        {
            throw new ArgumentNullException(nameof(serviceType));
        }

        return provider.GetService(serviceType);
    }
}
