using System.Threading;
using System.Threading.Tasks;

namespace Nut.MediatR;

/// <summary>
/// <see cref="AuthorizationBehavior{TRequest, TResponse}"/> から実行される認可処理のインターフェイスを定義します。
/// </summary>
/// <typeparam name="TRequest">リクエストの型</typeparam>
public interface IAuthorizer<TRequest>
{
    /// <summary>
    /// 認可処理を実行して結果を返します。
    /// </summary>
    /// <param name="request">リクエストの値</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>認可の結果</returns>
    public Task<AuthorizationResult> AuthorizeAsync(TRequest request, CancellationToken cancellationToken);
}
