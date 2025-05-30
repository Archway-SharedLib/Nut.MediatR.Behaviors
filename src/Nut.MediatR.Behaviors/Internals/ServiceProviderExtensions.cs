using System;
using System.Collections.Generic;
using System.Linq;

namespace Nut.MediatR.Internals;

internal static class ServiceProviderExtensions
{
    public static IEnumerable<T> GetServicesOrEmpty<T>(this IServiceProvider provider)
    {
        if (provider is null) throw new ArgumentNullException(nameof(provider));
        var enumerableType = typeof(IEnumerable<>).MakeGenericType(typeof(T));
        return provider.GetService(enumerableType) as IEnumerable<T> ?? Enumerable.Empty<T>();
    }

    public static T? GetFirstServiceOrDefault<T>(this IServiceProvider provider)
    {
        if (provider is null) throw new ArgumentNullException(nameof(provider));
        var instances = provider.GetServicesOrEmpty<T>();
        if (!instances.Any()) return default;
        return instances.First();
    }
}
