using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using SR = Nut.MediatR.ServiceLike.Resources.Strings;

#pragma warning disable 618

namespace Nut.MediatR.ServiceLike;

/// <summary>
/// イベントを受信するリスナーの定義を保持します。
/// </summary>
public class MediatorListenerDescription
{
    private MediatorListenerDescription(string key, Type listenerType)
    {
        Key = key;
        ListenerType = listenerType;
        MediateType = GetMediateTypeFrom(listenerType);
    }

    private MediateType GetMediateTypeFrom(Type listenerType)
        => listenerType.IsImplemented(typeof(INotification))
            ? MediateType.Notification
            : MediateType.Request;

    /// <summary>
    /// <see cref="Type"/> をもとに <see cref="MediatorListenerDescription"/> のインスタンスを作成します。
    /// </summary>
    /// <remarks>
    /// <paramref name="listenerType"/> で指定された型は <see cref="CanListenerize"/> で判定されるリスナーになれる型である必要があります。
    /// </remarks>
    /// <param name="listenerType">リスナーの型</param>
    /// <returns>型をもとにしたすべての <see cref="MediatorListenerDescription"/> </returns>
    public static IEnumerable<MediatorListenerDescription> Create(Type listenerType)
    {
        if (listenerType is null)
        {
            throw new ArgumentNullException(nameof(listenerType));
        }

        if (!CanListenerize(listenerType))
        {
            throw new ArgumentException(SR.Argument_CanNotListenerize(nameof(listenerType)));
        }

        var evListenerAttrs = listenerType.GetAttributes<AsEventListenerAttribute>(true).ToList();
        var paths = evListenerAttrs.Select(attr => attr.Key);

        return paths.Select(path =>
            new MediatorListenerDescription(path, listenerType)
        ).ToList();
    }

    /// <summary>
    /// リスナーの型を取得します。
    /// </summary>
    public Type ListenerType { get; }

    /// <summary>
    /// 仲介の形式を取得します。
    /// </summary>
    public MediateType MediateType { get; }

    /// <summary>
    /// キーを取得します。
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// 指定された型がリスナーになれるかどうかを判定します。
    /// </summary>
    /// <remarks>
    /// 次の条件に合致する場合にリスナーになれます。
    /// <list type="bullet">
    ///     <item>
    ///         <description>オープンジェネリックタイプではない</description>
    ///         <description><see cref="INotification"/>または<see cref="IRequest"/>または<see cref="IRequest{T}"/>を実装しているクラスである</description>
    ///         <description><see cref="AsEventListenerAttribute"/>が設定されている</description>
    ///     </item>
    /// </list>
    /// </remarks>
    /// <param name="listenerType">判定する型</param>
    /// <returns>指定された型がリスナーになれる場合は <see langword="true"/> 、そうでない場合は <see langword="false"/></returns>
    public static bool CanListenerize(Type listenerType)
        => !listenerType.IsOpenGeneric()
            && listenerType.IsConcrete()
            && (listenerType.IsImplemented(typeof(INotification))
                || listenerType.IsImplemented(typeof(IRequest))
                || listenerType.IsImplemented(typeof(IRequest<>)))
            && listenerType.GetAttributes<AsEventListenerAttribute>().Any();
}
