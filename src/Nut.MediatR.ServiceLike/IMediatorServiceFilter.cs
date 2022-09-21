using System;
using System.Threading.Tasks;

namespace Nut.MediatR.ServiceLike;

/// <summary>
/// メッセージの送信前後で処理を実行するためのフィルターが実装する必要のあるインターフェイスを定義します。
/// </summary>
public interface IMediatorServiceFilter
{
    /// <summary>
    /// メッセージの送信前後で処理を行います。
    /// </summary>
    /// <remarks>
    /// 処理はメッセージを送信する場合は、必ず <paramref name="next"/> を実行する必要があります。
    /// <paramref name="next"/> が実行されない場合、メッセージは送信されません。
    /// <code>
    /// <![CDATA[
    /// public Task<object> HandleAsync(RequestContext context, object? parameter, Func<object?, Task<object?>> next)
    /// {
    ///   // do something
    ///   return next(parameter);
    /// }
    /// ]]>
    /// </code>
    /// </remarks>
    /// <param name="context">リクエストの情報</param>
    /// <param name="parameter">メッセージのデータ</param>
    /// <param name="next">後続するフィルター処理あるいはメッセージの送信処理</param>
    /// <returns>メッセージの結果</returns>
    Task<object> HandleAsync(RequestContext context, object? parameter, Func<object?, Task<object?>> next);
}
