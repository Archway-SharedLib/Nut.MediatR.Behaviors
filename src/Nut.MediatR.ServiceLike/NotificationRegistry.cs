using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nut.MediatR.ServiceLike
{
    public class NotificationRegistry
    {
        private ConcurrentDictionary<string, ConcurrentBag<MediatorNotification>> notificationPool = new();

        public void Add(Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var notifications = MediatorNotification.Create(type);
            foreach (var notification in notifications)
            {
                var bag = notificationPool.GetOrAdd(notification.Key, key => new ConcurrentBag<MediatorNotification>());
                bag.Add(notification);
            }
        }

        public IEnumerable<string> GetKeys()
        {
            return notificationPool.Keys;
        }

        public IEnumerable<MediatorNotification> GetNotifications(string key)
        {
            if(notificationPool.TryGetValue(key, out var value))
            {
                return value!;
            }
            return Enumerable.Empty<MediatorNotification>();
        }
    }
}
