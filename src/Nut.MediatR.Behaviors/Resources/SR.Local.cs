//using System;
//using System.CodeDom.Compiler;
//using System.Collections.Generic;
//using System.Text;

//namespace Nut.MediatR.Resources
//{
//    internal static class Strings { }
//}

//namespace System
//{
//    internal static partial class SR
//    {
//        private static global::System.Resources.ResourceManager? s_resourceManager = null;
//        internal static global::System.Resources.ResourceManager ResourceManager => s_resourceManager ??= new global::System.Resources.ResourceManager(typeof(Nut.MediatR.Resources.Strings));

//        internal static string Authorization_NotAuthorized => GetResourceString(nameof(Authorization_NotAuthorized), @"Not authorized.");

//        internal static string PerRequest_ContainsNullInTypes => GetResourceString(nameof(PerRequest_ContainsNullInTypes), @"Contains null in behavior types.");

//        internal static string PerRequest_TypeIsNotBehavior(string sourceType, string behaviorType) => Format(GetResourceString(nameof(PerRequest_TypeIsNotBehavior), "{0} is not {1} type."), sourceType, behaviorType);
//    }
//}
