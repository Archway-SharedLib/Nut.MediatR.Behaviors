using System.Threading;
using System.Threading.Tasks;

namespace Nut.MediatR.Logging;

/// <summary>
/// <see cref="ILoggingInOutValueCollector{TRequest, TResponse}"/> で追加の値を返さない実装を提供します。
/// 入力または出力のどちらかのみ追加の値を指定する場合にメソッドをオーバーライドして利用します。
/// </summary>
/// <typeparam name="TRequest">リクエストの型</typeparam>
/// <typeparam name="TResponse">レスポンスの型</typeparam>
public abstract class BaseLoggingInOutValueCollector<TRequest, TResponse> : ILoggingInOutValueCollector<TRequest, TResponse>
    where TRequest : notnull
{
    /// <inheritdoc />
    public virtual Task<InOutValueResult> CollectInValueAsync(TRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(InOutValueResult.Empty());
    }

    /// <inheritdoc />
    public virtual Task<InOutValueResult> CollectOutValueAsync(TResponse response, CancellationToken cancellationToken)
    {
        return Task.FromResult(InOutValueResult.Empty());
    }
}
