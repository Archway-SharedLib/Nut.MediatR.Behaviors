using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Nut.MediatR
{
    public class PerRequestBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly ServiceFactory factory;

        public PerRequestBehavior(ServiceFactory factory)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var types = Cache<TRequest, TResponse>.Types.ToList();
            return await ExecuteBehaviors(types, request, cancellationToken, next);
        }

        private async Task<TResponse> ExecuteBehaviors(IList<Type> types, TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (!types.Any()) return await next.Invoke();
            var type = types[0];
            var service = factory(type) as IPipelineBehavior<TRequest, TResponse>;
            types.RemoveAt(0);
            if (service == null)
            {
                return await ExecuteBehaviors(types, request, cancellationToken, next);
            }
            return await service.Handle(request, cancellationToken, async () => await ExecuteBehaviors(types, request, cancellationToken, next));
        }


        private static class Cache<TReqCache, TResCache>
        {
            public static Type[] Types { get; } = GetBehaviorTypesFromAttribute();

            private static Type[] GetBehaviorTypesFromAttribute()
            {
                var targetType = typeof(TRequest);
                if (!(targetType.GetCustomAttribute(typeof(WithBehaviorsAttribute), true) is WithBehaviorsAttribute attribute))
                    return new Type[0];
                return attribute.BehaviorTypes.Select(t => t.MakeGenericType(typeof(TReqCache), typeof(TResCache))).ToArray();
            }
        }
    }
}
