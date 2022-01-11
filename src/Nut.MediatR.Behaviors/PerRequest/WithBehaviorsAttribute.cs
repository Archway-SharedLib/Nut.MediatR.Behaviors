using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MediatR;
using SR = Nut.MediatR.Resources.Strings;

namespace Nut.MediatR;

/// <summary>
/// <see cref="PerRequestBehavior{TRequest, TResponse}"/> を通して実行される <see cref="IPipelineBehavior{TRequest, TResponse}"/> を指定します。
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true, AllowMultiple = false)]
public class WithBehaviorsAttribute : Attribute
{
    /// <summary>
    /// 実行する <see cref="IPipelineBehavior{TRequest, TResponse}"/> の <see cref="Type"/> を取得します。
    /// </summary>
    public IList<Type> BehaviorTypes { get; }

    /// <summary>
    /// インスタンスを初期化します。
    /// </summary>
    /// <param name="behaviorTypes">実行する <see cref="IPipelineBehavior{TRequest, TResponse}"/> の <see cref="Type"/></param>
    public WithBehaviorsAttribute(params Type[] behaviorTypes)
    {
        if (behaviorTypes is null) throw new ArgumentNullException(nameof(behaviorTypes));
        foreach (var type in behaviorTypes)
        {
            if (type is null) throw new ArgumentException(SR.PerRequest_ContainsNullInTypes);
            if (!IsPipelineBehaviorType(type)) throw new ArgumentException(SR.PerRequest_TypeIsNotBehavior(type.FullName, typeof(IPipelineBehavior<,>).Name));
        }
        BehaviorTypes = new ReadOnlyCollection<Type>(behaviorTypes);
    }

    private bool IsPipelineBehaviorType(Type behaviorType)
        => behaviorType
            .GetInterfaces()
            .Where(i => i.IsGenericType)
            .Any(i => i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>));
}
