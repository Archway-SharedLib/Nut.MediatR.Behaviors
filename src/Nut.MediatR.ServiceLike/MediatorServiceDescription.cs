using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using SR = Nut.MediatR.ServiceLike.Resources.Strings;

namespace Nut.MediatR.ServiceLike;

/// <summary>
/// メッセージを受け取るするサービスの定義を保持します。
/// </summary>
public class MediatorServiceDescription
{
    private MediatorServiceDescription(string path, Type serviceType, IEnumerable<Type> filters)
    {
        Path = path;
        ServiceType = serviceType;
        Filters = filters;
    }

    /// <summary>
    /// <see cref="Type"/> をもとに <see cref="MediatorServiceDescription"/> のインスタンスを作成します。
    /// </summary>
    /// <remarks>
    /// <paramref name="serviceType"/> で指定された型は <see cref="CanServicalize"/> で判定されるサービスになれる型である必要があります。
    /// </remarks>
    /// <param name="serviceType">サービスの型</param>
    /// <param name="filterTypes">前後処理を行うフィルターの型</param>
    /// <returns>型をもとにしたすべての <see cref="MediatorServiceDescription"/> </returns>
    public static IEnumerable<MediatorServiceDescription> Create(Type serviceType, params Type[] filterTypes)
    {
        if (serviceType is null)
        {
            throw new ArgumentNullException(nameof(serviceType));
        }
        FilterSupport.ThrowIfInvalidFilterTypeAllWith(filterTypes);

        if (!CanServicalize(serviceType))
        {
            throw new ArgumentException(SR.Argument_CanNotServicalize(nameof(serviceType)));
        }
        var attrs = serviceType.GetAttributes<AsServiceAttribute>(true);
        return attrs.Select(attr =>
        {
            var attrFilters = attr.FilterTypes;
            var filters = filterTypes.Concat(attrFilters).ToList();
            return new MediatorServiceDescription(attr.Path, serviceType, filters);
        }).ToList();
    }

    /// <summary>
    /// サービスの型を取得する
    /// </summary>
    public Type ServiceType { get; }

    /// <summary>
    /// フィルターの型を取得する
    /// </summary>
    public IEnumerable<Type> Filters { get; }

    /// <summary>
    /// パスを取得する
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// 指定された型がサービスになれるかどうかを判定します。
    /// </summary>
    /// <remarks>
    /// 次の条件に合致する場合にサービスになれます。
    /// <list type="bullet">
    ///     <item>
    ///         <description>オープンジェネリックタイプではない</description>
    ///         <description><see cref="IRequest"/>または<see cref="IRequest{T}"/>を実装しているクラスである</description>
    ///         <description><see cref="AsServiceAttribute"/>が設定されている</description>
    ///     </item>
    /// </list>
    /// </remarks>
    /// <param name="requestType">判定する型</param>
    /// <returns>指定された型がサービスになれる場合は <see langword="true"/> 、そうでない場合は <see langword="false"/></returns>
    public static bool CanServicalize(Type requestType)
        => !requestType.IsOpenGeneric()
            && requestType.IsConcrete()
            && (requestType.IsImplemented(typeof(IRequest<>))
                || requestType.IsImplemented(typeof(IRequest)))
            && requestType.GetAttributes<AsServiceAttribute>().Any();
}
