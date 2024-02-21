using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Nut.MediatR;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection TryAddTransientGenericInterfaceTypeFromAssemblies(this IServiceCollection source, Assembly[] assemblies, Type serviceType)
    {
        foreach (var assembly in assemblies)
        {
            foreach (var target in assembly.DefinedTypes.Where(t => !t.IsGenericType))
            {
                var interfaceType = target
                    .GetInterfaces()
                    .Where(i => i.IsGenericType)
                    .FirstOrDefault(i => i.GetGenericTypeDefinition() == serviceType);
                if (interfaceType != null)
                {
                    source.TryAddTransient(interfaceType, target);
                }
            }
        }
        
        return source;
    }
}
