using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Nut.MediatR.Internals;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection TryAddTransientGenericInterfaceTypeFromAssemblies(this IServiceCollection source, Assembly[] assemblies, Type serviceType)
    {
        foreach (var targetType in assemblies.SelectMany(a => a.DefinedTypes).Where(t => !t.IsGenericType))
        {
            var interfaceType = targetType
                .GetInterfaces()
                .Where(i => i.IsGenericType)
                .FirstOrDefault(i => i.GetGenericTypeDefinition() == serviceType);
            if (interfaceType != null)
            {
                source.TryAddTransient(interfaceType, targetType);
            }
        }

        return source;
    }
}
