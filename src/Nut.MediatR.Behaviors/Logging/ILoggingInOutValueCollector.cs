using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Nut.MediatR;

/// <summary>
/// ログに出力する追加の値を取得するためのインターフェイスを定義します。
/// </summary>
/// <typeparam name="TRequest">リクエストの型</typeparam>
/// <typeparam name="TResponse">レスポンスの型</typeparam>
public interface ILoggingInOutValueCollector<in TRequest, TResponse> where TRequest : notnull
{
    /// <summary>
    /// 入力時に出力する追加の値を取得します。
    /// </summary>
    /// <param name="request">入力された値</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>入力時に出力する追加の値</returns>
    Task<InOutValueResult> CollectInValueAsync(TRequest request, CancellationToken cancellationToken)
        => Task.FromResult(InOutValueResult.Empty());

    /// <summary>
    /// 出力時に出力する追加の値を取得します。
    /// </summary>
    /// <param name="response">出力された値</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>出力時に出力する追加の値</returns>
    Task<InOutValueResult> CollectOutValueAsync(TResponse response, CancellationToken cancellationToken)
        => Task.FromResult(InOutValueResult.Empty());
}

/// <summary>
/// ログに出力する追加の値を取得するためのインターフェイスを定義します。
/// </summary>
/// <typeparam name="TRequest">リクエストの型</typeparam>
public interface ILoggingInOutValueCollector<in TRequest> : ILoggingInOutValueCollector<TRequest, Unit> where TRequest : notnull
{
}
