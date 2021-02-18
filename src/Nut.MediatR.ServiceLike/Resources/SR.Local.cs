using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;

namespace Nut.MediatR.ServiceLike.Resources
{
    internal static class Strings { }
}

namespace System
{
    internal static partial class SR
    {
        private static global::System.Resources.ResourceManager? s_resourceManager = null;
        internal static global::System.Resources.ResourceManager ResourceManager => s_resourceManager ??= new global::System.Resources.ResourceManager(typeof(Nut.MediatR.ServiceLike.Resources.Strings));

        internal static string Argument_CanNotNullOrWhitespace(string paramName) => Format(GetResourceString(nameof(Argument_CanNotNullOrWhitespace), "{0} cannot be null or empty."), paramName);

        internal static string MediatorRequestNotFound(string path) => Format(GetResourceString(nameof(MediatorRequestNotFound), "Mediator request was not found. path: {0}"), path);

        internal static string FilterTypeConstratins => GetResourceString(nameof(FilterTypeConstratins), @"FilterType must implement the IMediatorServiceFilter and have a default constructor.");

        internal static string Argument_CanNotServicalize(string paramName) => Format(GetResourceString(nameof(Argument_CanNotServicalize), "{0} argument must be implemented as a closed generic type of IRequest(<T>) with AsServiceAttribute."), paramName);

        internal static string Registry_AlreadyContainsPath(string path) => Format(GetResourceString(nameof(Registry_AlreadyContainsPath), "Already contains path: {0}"), path);

        internal static string Argument_CanNotListenerize(string paramName) => Format(GetResourceString(nameof(Argument_CanNotListenerize), "{0} argument must be implemented as a closed generic type of INotification or IReuqest(<T>) with AsEventListenerAttribute."), paramName);

        internal static string Registry_AlreadyContainsKey(string key) => Format(GetResourceString(nameof(Registry_AlreadyContainsKey), "Already contains key: {0}"), key);

        internal static string Client_RaiseExWhenPublish(string notificationType) => Format(GetResourceString(nameof(Client_RaiseExWhenPublish), "Raise exception when publish: {0}."), notificationType);
   
    }
}
