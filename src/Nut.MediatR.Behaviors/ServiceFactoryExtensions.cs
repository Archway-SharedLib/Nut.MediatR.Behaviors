using System.Linq;
using MediatR;

namespace Nut.MediatR;

internal static class ServiceFactoryExtensions
{
    public static T? GetInstanceOrDefault<T>(this ServiceFactory factory)
    {
        return GetInstanceOrDefault<T>(factory, default);
    }

    public static T? GetInstanceOrDefault<T>(this ServiceFactory factory, T? defaultValue)
    {
        var instances = factory.GetInstances<T>();
        if (instances is null || !instances.Any()) return defaultValue;
        return instances.First();
    }
}
