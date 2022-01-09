using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Nut.MediatR.ServiceLike;

public class ListenerRegistry
{
    private readonly ConcurrentDictionary<string, ConcurrentBag<MediatorListenerDescription>> listenerPool = new();

    public void Add(Type type)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        var listeners = MediatorListenerDescription.Create(type);
        foreach (var listener in listeners)
        {
            var bag = listenerPool.GetOrAdd(listener.Key, key => new ConcurrentBag<MediatorListenerDescription>());
            bag.Add(listener);
        }
    }

    public IEnumerable<string> GetKeys() => listenerPool.Keys;

    public IEnumerable<MediatorListenerDescription> GetListeners(string key)
        => listenerPool.TryGetValue(key, out var value) ? value : Enumerable.Empty<MediatorListenerDescription>();
}
