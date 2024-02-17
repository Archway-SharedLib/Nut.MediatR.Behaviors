using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SR = Nut.MediatR.Resources.Strings;

namespace Nut.MediatR;

/// <summary>
/// 認可を行う <see cref="IPipelineBehavior{TRequest, TResponse}"/> の実装を定義します。
/// </summary>
/// <typeparam name="TRequest">リクエストの型</typeparam>
/// <typeparam name="TResponse">レスポンスの型</typeparam>
public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    /// <summary>
    /// <see cref="ServiceProvider"/> を取得します。
    /// </summary>
    protected IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// インスタンスを初期化します。
    /// </summary>
    /// <param name="serviceFactory">サービスを取得する <see cref="IServiceProvider"/></param>
    public AuthorizationBehavior(IServiceProvider serviceFactory)
    {
        ServiceProvider = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
    }

    /// <summary>
    /// <see cref="IAuthorizer{TRequest}"/> の実装を取得して実行します。
    /// </summary>
    /// <param name="request">リクエスト</param>
    /// <param name="next">次の処理</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>処理結果</returns>
    /// <exception cref="UnauthorizedException">認可処理が失敗した場合に発生します。</exception>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var authorizers = GetAuthorizers()?.ToList();
        if (authorizers?.Any() == true)
        {
            foreach (var authorizer in authorizers)
            {
                var result = await authorizer.AuthorizeAsync(request, cancellationToken).ConfigureAwait(false);
                if (!result.Succeeded) throw new UnauthorizedException(
                    string.IsNullOrEmpty(result.FailureMessage) ? SR.Authorization_NotAuthorized : result.FailureMessage);
            }
        }

        return await next().ConfigureAwait(false);
    }

    /// <summary>
    /// 実行する <see cref="IAuthorizer{TRequest}"/> を取得します。
    /// </summary>
    /// <returns>サービスとして登録されている<see cref="IAuthorizer{TRequest}"/></returns>
    protected virtual IEnumerable<IAuthorizer<TRequest>> GetAuthorizers()
    {
        return GetRegisteredAuthorizers();
    }

    private IEnumerable<IAuthorizer<TRequest>> GetRegisteredAuthorizers()
    {
        return ServiceProvider.GetServicesOrEmpty<IAuthorizer<TRequest>>();
    }
}
