using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using SR = Nut.MediatR.ServiceLike.Resources.Strings;

namespace Nut.MediatR.ServiceLike;

public class ServiceRegistry
{
    private readonly ConcurrentDictionary<string, MediatorServiceDescription> servicePool = new();

    public void Add(Type type, params Type[] filterTypes)
    {
        Add(type, false, filterTypes);
    }

    public void Add(Type type, bool ignoreDuplication, params Type[] filterTypes)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }
        FilterSupport.ThrowIfInvalidFilterTypeAllWith(filterTypes);

        var services = MediatorServiceDescription.Create(type, filterTypes);
        foreach (var service in services)
        {
            if (!servicePool.TryAdd(service.Path, service))
            {
                if (!ignoreDuplication)
                {
                    throw new ArgumentException(SR.Registry_AlreadyContainsPath(service.Path), nameof(type));
                }
            }
        }
    }

    public IEnumerable<string> GetEndpoints() => servicePool.Keys;

    public MediatorServiceDescription? GetService(string path)
        => servicePool.TryGetValue(path, out var value) ? value : null;
}
