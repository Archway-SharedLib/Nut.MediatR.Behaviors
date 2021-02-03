using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Nut.MediatR.ServiceLike
{
    public class EventRegistry
    {
        private ConcurrentDictionary<string, MediatorEvent> eventPool = new ConcurrentDictionary<string, MediatorEvent>();

        public void Add(Type type)
        {
            Add(type, false);
        }

        public void Add(Type type, bool ignoreDuplication)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var events = MediatorEvent.Create(type);
            foreach (var @event in events)
            {
                if (!eventPool.TryAdd(@event.Key, @event))
                {
                    if(!ignoreDuplication)
                    {
                        throw new ArgumentException(SR.Registry_AlreadyContainsPath(@event.Key), nameof(type));
                    }
                }
            }
        }

        public IEnumerable<string> GetKeys()
        {
            return eventPool.Keys;
        }

        public MediatorEvent? GetEvent(string key)
        {
            if(eventPool.TryGetValue(key, out var value))
            {
                return value;
            }
            return null;
        }
    }
}
