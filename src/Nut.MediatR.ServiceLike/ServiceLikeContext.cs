using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SR = Nut.MediatR.ServiceLike.Resources.Strings;

namespace Nut.MediatR.ServiceLike;

/// <summary>
/// サービスのコンテキスト情報を保持します。
/// </summary>
public class ServiceLikeContext : IServiceLikeContext
{
    /// <summary>
    /// インスタンスを初期化します。
    /// </summary>
    /// <param name="key">サービスのキー</param>
    public ServiceLikeContext(string key) : this(key, null) { }

    /// <summary>
    /// インスタンスを初期化します。
    /// </summary>
    /// <param name="key">サービスのキー</param>
    /// <param name="header">保続情報</param>
    public ServiceLikeContext(string key, IDictionary<string, object>? header)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException(SR.Argument_CanNotNullOrWhitespace(nameof(key)));
        }

        Key = key;
        Header = new ReadOnlyDictionary<string, object>(header ?? new Dictionary<string, object>());
        Id = Guid.NewGuid().ToString("N");
        Timestamp = DateTimeOffset.Now.Ticks;
    }

    /// <inheritdoc />
    public string Id { get; }

    /// <inheritdoc />
    public string Key { get; }

    /// <inheritdoc />
    public IDictionary<string, object> Header { get; }

    /// <inheritdoc />
    public long Timestamp { get; }
}
