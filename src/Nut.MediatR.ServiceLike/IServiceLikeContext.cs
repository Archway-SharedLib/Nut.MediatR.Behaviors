using System.Collections.Generic;

namespace Nut.MediatR.ServiceLike;

/// <summary>
/// サービスのコンテキスト情報を保持します。
/// </summary>
public interface IServiceLikeContext
{
    /// <summary>
    /// IDを取得します。
    /// </summary>
    string Id { get; }

    /// <summary>
    /// サービスのキーを取得します。
    /// </summary>
    string Key { get; }

    /// <summary>
    /// 付属情報を取得します。
    /// </summary>
    IDictionary<string, object> Header { get; }

    /// <summary>
    /// 実行されたタイムスタンプを取得します。
    /// </summary>
    long Timestamp { get; }
}
