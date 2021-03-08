using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SR = Nut.MediatR.ServiceLike.Resources.Strings;

#pragma warning disable 618

namespace Nut.MediatR.ServiceLike
{
    public class MediatorListenerDescription
    {
        private MediatorListenerDescription(string key, Type listenerType)
        {
            Key = key;
            ListenerType = listenerType;
            MediateType = GetMediateTypeFrom(listenerType);
        }

        private MediateType GetMediateTypeFrom(Type listenerType)
            => listenerType.IsImplemented(typeof(INotification))
                ? MediateType.Notification
                : MediateType.Request;

        public static IEnumerable<MediatorListenerDescription> Create(Type listenerType)
        {
            if (listenerType is null)
            {
                throw new ArgumentNullException(nameof(listenerType));
            }

            if (!CanListenerize(listenerType))
            {
                throw new ArgumentException(SR.Argument_CanNotListenerize(nameof(listenerType)));
            }

            var evListenerAttrs = listenerType.GetAttributes<AsEventListenerAttribute>(true).ToList();
            var paths = evListenerAttrs.Any()
                ? evListenerAttrs.Select(attr => attr.Path)
                : listenerType.GetAttributes<AsEventAttribute>(true).Select(attr => attr.Path);
            
            return paths.Select(path => 
                new MediatorListenerDescription(path, listenerType)
            ).ToList();
        }

        public Type ListenerType { get; }
        
        public MediateType MediateType { get; }
        
        public string Key { get; }

        public static bool CanListenerize(Type listenerType)
            => !listenerType.IsOpenGeneric()
                && listenerType.IsConcrete()
                && (listenerType.IsImplemented(typeof(INotification)) 
                    || listenerType.IsImplemented(typeof(IRequest)) 
                    || listenerType.IsImplemented(typeof(IRequest<>)))
                && (listenerType.GetAttributes<AsEventListenerAttribute>().Any() 
                    || listenerType.GetAttributes<AsEventAttribute>().Any());
    }
}