using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Nut.MediatR.ServiceLike
{
    internal static class TypeExtensions
    {
        public static bool IsOpenGeneric(this Type type)
            => type.IsGenericTypeDefinition || type.ContainsGenericParameters;

        public static bool IsConcrete(this Type type)
            => !type.IsAbstract && !type.IsInterface;

        public static TAttr? GetAttribute<TAttr>(this Type type, bool inherit) where TAttr: Attribute
            => type.GetCustomAttributes(typeof(TAttr), inherit).FirstOrDefault() as TAttr;

        public static TAttr? GetAttribute<TAttr>(this Type type) where TAttr : Attribute
            => type.GetCustomAttributes(typeof(TAttr)).FirstOrDefault() as TAttr;

        public static IEnumerable<TAttr> GetAttributes<TAttr>(this Type type, bool inherit) where TAttr : Attribute
            => type.GetCustomAttributes(typeof(TAttr), inherit).Cast<TAttr>();

        public static IEnumerable<TAttr> GetAttributes<TAttr>(this Type type) where TAttr : Attribute
            => type.GetCustomAttributes(typeof(TAttr)).Cast<TAttr>();

        public static bool IsImplemented(this Type type, Type interfaceType)
        {
            if (type.IsInterface) return false;
            if (!interfaceType.IsInterface) return false;
            var isOpenInterface = interfaceType.IsOpenGeneric();
            return type.GetInterfaces().Any(i =>
            {
                if(isOpenInterface && i.IsGenericType)
                {
                    return i.GetGenericTypeDefinition() == interfaceType;
                }
                return i == interfaceType;
            });
        }

        public static bool HasDefaultConstructor(this Type type) => GetDefaultConstructor(type) != null;

        public static ConstructorInfo GetDefaultConstructor(this Type type) => type.GetConstructor(Type.EmptyTypes);

        private static ConcurrentDictionary<Type, Func<object>> activatorCache = new ConcurrentDictionary<Type, Func<object>>();

        public static T Activate<T>(this Type type)
        {
            var expr = activatorCache.GetOrAdd(type, (t) =>
            {
                return Expression.Lambda<Func<object>>(Expression.New(type.GetDefaultConstructor())).Compile();
            });
            return (T)expr.Invoke();
        }
    }
}
