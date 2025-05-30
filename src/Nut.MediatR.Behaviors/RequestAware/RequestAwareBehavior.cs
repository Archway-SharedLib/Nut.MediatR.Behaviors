using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Nut.MediatR;

/// <summary>
/// <see cref="IRequest"/> / <see cref="IRequest{TResponse}"/> ごとに指定された <see cref="IPipelineBehavior{TRequest, TResponse}"/> を実行します。
/// </summary>
/// <typeparam name="TRequest">リクエストの型</typeparam>
/// <typeparam name="TResponse">レスポンスの型</typeparam>
public class RequestAwareBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IServiceProvider _provider;

    /// <summary>
    /// <see cref="IServiceProvider"/> を指定してインスタンスを初期化します。
    /// </summary>
    /// <param name="provider">サービスを取得する <see cref="IServiceProvider"/></param>
    public RequestAwareBehavior(IServiceProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    /// <summary>
    /// <see cref="IRequest"/> / <see cref="IRequest{TResponse}"/> ごとに指定された <see cref="IPipelineBehavior{TRequest, TResponse}"/> を実行して、処理を実行します。
    /// </summary>
    /// <param name="request">リクエスト</param>
    /// <param name="next">次の処理</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>処理結果</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var types = Cache<TRequest, TResponse>.Types.ToList();
        return await ExecuteBehaviors(types, request, next, cancellationToken).ConfigureAwait(false);
    }

    private async Task<TResponse> ExecuteBehaviors(IList<Type> types, TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!types.Any()) return await next.Invoke(cancellationToken).ConfigureAwait(false);
        var type = types[0];
        types.RemoveAt(0);
        if (_provider.GetService(type) is not IPipelineBehavior<TRequest, TResponse> service)
        {
            return await ExecuteBehaviors(types, request, next, cancellationToken).ConfigureAwait(false);
        }
        return await service.Handle(request,
            async (ct) => await ExecuteBehaviors(types, request, next, ct).ConfigureAwait(false),
            cancellationToken
        ).ConfigureAwait(false);
    }

    private static class Cache<TReqCache, TResCache>
    {
        public static Type[] Types { get; } = GetBehaviorTypesFromAttribute();

        private static Type[] GetBehaviorTypesFromAttribute()
        {
            var targetType = typeof(TRequest);
            if (targetType.GetCustomAttribute(typeof(WithBehaviorsAttribute), true) is not WithBehaviorsAttribute attribute)
                return new Type[0];
            return attribute.BehaviorTypes.Select(t => t.MakeGenericType(typeof(TReqCache), typeof(TResCache))).ToArray();
        }
    }
}
