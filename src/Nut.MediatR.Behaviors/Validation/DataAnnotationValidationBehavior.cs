using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Nut.MediatR;

/// <summary>
/// <see cref="System.ComponentModel.DataAnnotations"/> で指定されたバリデーションを実行します。
/// </summary>
/// <typeparam name="TRequest">リクエストの型</typeparam>
/// <typeparam name="TResponse">レスポンスの型</typeparam>
public class DataAnnotationValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// インスタンスを初期化します。
    /// </summary>
    public DataAnnotationValidationBehavior()
    {
    }

    /// <summary>
    /// バリデーションを実行します。
    /// </summary>
    /// <param name="request">リクエストの値</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <param name="next">次の処理</param>
    /// <returns>処理の結果</returns>
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        Validator.ValidateObject(request, new ValidationContext(request), true);
        return await next().ConfigureAwait(false);
    }
}
