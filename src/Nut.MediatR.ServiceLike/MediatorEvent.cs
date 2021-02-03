using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nut.MediatR.ServiceLike
{
    public class MediatorEvent
    {
        private MediatorEvent(string key, Type eventType)
        {
            Key = key;
            EventType = eventType;
        }

        public static IEnumerable<MediatorEvent> Create(Type eventType)
        {
            if (eventType is null)
            {
                throw new ArgumentNullException(nameof(eventType));
            }

            if (!CanEventalize(eventType))
            {
                throw new ArgumentException(SR.Argument_CanNotEventalize(nameof(eventType)));
            }
            var attrs = eventType.GetAttributes<AsEventAttribute>(true);
            return attrs.Select(attr => 
            {
                return new MediatorEvent(attr.Path, eventType);
            }).ToList();
        }

        public Type EventType { get; }
        
        public string Key { get; }

        public static bool CanEventalize(Type requestType)
            => !requestType.IsOpenGeneric()
                && requestType.IsConcrete()
                && requestType.IsImplemented(typeof(INotification))
                && requestType.GetAttributes<AsEventAttribute>().Any();
    }
}