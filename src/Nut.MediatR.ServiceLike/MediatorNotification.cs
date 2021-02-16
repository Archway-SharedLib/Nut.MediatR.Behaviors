using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#pragma warning disable 618

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

            var evListenerAttrs = notificationType.GetAttributes<AsEventListenerAttribute>(true).ToList();
            var paths = evListenerAttrs.Any()
                ? evListenerAttrs.Select(attr => attr.Path)
                : notificationType.GetAttributes<AsEventAttribute>(true).Select(attr => attr.Path);
            
            return paths.Select(path => 
                new MediatorNotification(path, notificationType)
            ).ToList();
        }

        public Type NotificationType { get; }
        
        public string Key { get; }

        public static bool CanEventalize(Type requestType)
            => !requestType.IsOpenGeneric()
                && requestType.IsConcrete()
                && requestType.IsImplemented(typeof(INotification))
                && (requestType.GetAttributes<AsEventListenerAttribute>().Any() || requestType.GetAttributes<AsEventAttribute>().Any());
    }
}