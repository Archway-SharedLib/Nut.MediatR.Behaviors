using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nut.MediatR.ServiceLike;

/// <summary>
/// メッセージを跛行する際のオプションを定義します。
/// </summary>
public class PublishOptions
{
    /// <summary>
    /// メッセージの付属情報を取得します。
    /// </summary>
    public IDictionary<string, object> Header { get; } = new Dictionary<string, object>();

    /// <summary>
    /// メッセージが発行される前に実行する処理を取得または設定します。
    /// </summary>
    public Func<object, IServiceLikeContext, Task>? BeforePublishAsyncHandler { get; set; }

    /// <summary>
    /// メッセージの発行が成功した場合に実行される処理を取得または設定します。
    /// </summary>
    public Func<object, IServiceLikeContext, Task>? CompleteAsyncHandler { get; set; }

    /// <summary>
    /// メッセージの発行が失敗した場合に実行される処理を取得または設定します。
    /// </summary>
    public Func<Exception, object, IServiceLikeContext, Task>? ErrorAsyncHandler { get; set; }
}
