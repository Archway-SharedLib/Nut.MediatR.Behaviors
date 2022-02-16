using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SR = Nut.MediatR.ServiceLike.Resources.Strings;

namespace Nut.MediatR.ServiceLike;

/// <summary>
/// 外部からリクエストを受信できるように設定します。
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class AsServiceAttribute : Attribute
{
    /// <summary>
    /// インスタンスを初期化します。
    /// </summary>
    /// <param name="path">特定するためのパスを指定します。</param>
    /// <param name="filterTypes">リクエストの前後で実行されるフィルターの <see cref="Type"/> を指定します。</param>
    public AsServiceAttribute(string path, params Type[] filterTypes)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException(SR.Argument_CanNotNullOrWhitespace(nameof(path)));
        }

        Path = path;

        if (filterTypes is null) throw new ArgumentNullException(nameof(filterTypes));
        FilterSupport.ThrowIfInvalidFilterTypeAllWith(filterTypes);

        FilterTypes = new ReadOnlyCollection<Type>(filterTypes);
    }

    /// <summary>
    /// パスを取得します。
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// 設定されているフィルターの <see cref="Type"/> を取得します。
    /// </summary>
    public IList<Type> FilterTypes { get; }
}
