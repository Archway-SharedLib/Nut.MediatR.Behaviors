using System;
using SR = Nut.MediatR.ServiceLike.Resources.Strings;

namespace Nut.MediatR.ServiceLike;

/// <summary>
/// メッセージがパブリッシュされたときに受信するリスナーとして設定します。
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class AsEventListenerAttribute : Attribute
{
    /// <summary>
    /// インスタンスを初期化します。
    /// </summary>
    /// <param name="key">イベントのキー名</param>
    /// <exception cref="ArgumentException"><paramref name="key"/>がnullまたは空/空白文字列の場合に発生します。</exception>
    public AsEventListenerAttribute(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException(SR.Argument_CanNotNullOrWhitespace(nameof(key)));
        }

        Key = key;
    }
    /// <summary>
    /// イベントのキー名を取得します。
    /// </summary>
    public string Key { get; }
}
