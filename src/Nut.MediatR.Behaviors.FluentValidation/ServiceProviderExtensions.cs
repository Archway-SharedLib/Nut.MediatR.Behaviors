using System;
using System.Collections.Generic;
using System.Linq;

namespace Nut.MediatR;

internal static class ServiceProviderExtensions
{
    public static IEnumerable<T> GetServicesOrEmpty<T>(this IServiceProvider provider)
    {
        if (provider is null) throw new ArgumentNullException(nameof(provider));

        var enumerableType = typeof(IEnumerable<>).MakeGenericType(typeof(T));
        return provider.GetService(enumerableType) as IEnumerable<T> ?? Enumerable.Empty<T>();
    }
}
