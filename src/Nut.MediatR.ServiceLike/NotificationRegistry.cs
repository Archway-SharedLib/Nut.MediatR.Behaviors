using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Nut.MediatR.ServiceLike
{
    public class NotificationRegistry
    {
        private ConcurrentDictionary<string, MediatorNotification> eventPool = new ConcurrentDictionary<string, MediatorNotification>();

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

            var notifications = MediatorNotification.Create(type);
            foreach (var notification in notifications)
            {
                if (!eventPool.TryAdd(notification.Key, notification))
                {
                    if(!ignoreDuplication)
                    {
                        throw new ArgumentException(SR.Registry_AlreadyContainsKey(notification.Key), nameof(type));
                    }
                }
            }
        }

        public IEnumerable<string> GetKeys()
        {
            return eventPool.Keys;
        }

        public MediatorNotification? GetNotification(string key)
        {
            if(eventPool.TryGetValue(key, out var value))
            {
                return value;
            }
            return null;
        }
    }
}
