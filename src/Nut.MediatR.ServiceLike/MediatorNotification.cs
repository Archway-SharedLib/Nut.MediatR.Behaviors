using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nut.MediatR.ServiceLike
{
    public class MediatorNotification
    {
        private MediatorNotification(string key, Type notificationType)
        {
            Key = key;
            NotificationType = notificationType;
        }

        public static IEnumerable<MediatorNotification> Create(Type notificationType)
        {
            if (notificationType is null)
            {
                throw new ArgumentNullException(nameof(notificationType));
            }

            if (!CanEventalize(notificationType))
            {
                throw new ArgumentException(SR.Argument_CanNotEventalize(nameof(notificationType)));
            }
            var attrs = notificationType.GetAttributes<AsEventAttribute>(true);
            return attrs.Select(attr => 
            {
                return new MediatorNotification(attr.Path, notificationType);
            }).ToList();
        }

        public Type NotificationType { get; }
        
        public string Key { get; }

        public static bool CanEventalize(Type requestType)
            => !requestType.IsOpenGeneric()
                && requestType.IsConcrete()
                && requestType.IsImplemented(typeof(INotification))
                && requestType.GetAttributes<AsEventAttribute>().Any();
    }
}