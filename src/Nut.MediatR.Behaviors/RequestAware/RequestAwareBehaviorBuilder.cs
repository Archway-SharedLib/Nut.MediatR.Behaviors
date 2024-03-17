using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Nut.MediatR;

/// <summary>
/// <see cref="RequestAwareBehavior{TRequest, TResponse}"/> を追加するためのビルダーを定義します。
/// </summary>
public class RequestAwareBehaviorBuilder
{
    /// <summary>
    /// インスタンスを初期化します。
    /// </summary>
    /// <param name="services"></param>
    public RequestAwareBehaviorBuilder(IServiceCollection services)
    {
        Services = services;
    }

    /// <summary>
    /// <see cref="IServiceCollection"/>
    /// </summary>
    public IServiceCollection Services { get; }

    private readonly List<Type> _openBehaviorTypes = new();

    private readonly List<Action<IServiceCollection, Assembly[]>> _autoRegistrationHandlers = new();

    private readonly List<Assembly> _assemblies = new();

    /// <summary>
    /// <see cref="RequestAwareBehavior{TRequest, TResponse}"/>で利用するための <see cref="IPipelineBehavior{TRequest, TResponse}"/> を追加します。
    /// </summary>
    /// <param name="openBehaviorType">追加する<see cref="IPipelineBehavior{TRequest, TResponse}"/>をオープンタイプで指定します。</param>
    /// <returns><see cref="RequestAwareBehaviorBuilder"/></returns>
    /// <exception cref="InvalidOperationException">指定された型がオープンタイプの<see cref="IPipelineBehavior{TRequest, TResponse}"/>ではない場合に発生します。</exception>
    public RequestAwareBehaviorBuilder AddOpenBehavior(Type openBehaviorType)
    {
        if (!openBehaviorType.IsGenericType)
        {
            throw new InvalidOperationException($"{openBehaviorType.Name} must be generic");
        }

        var implementedInterfaces = openBehaviorType.GetInterfaces()
            .Where(i => i.IsGenericType)
            .Select(i => i.GetGenericTypeDefinition())
            .Where(i => i == typeof(IPipelineBehavior<,>));

        if (!implementedInterfaces.Any())
        {
            throw new InvalidOperationException($"{openBehaviorType.Name} must implement {typeof(IPipelineBehavior<,>).FullName}");
        }

        _openBehaviorTypes.Add(openBehaviorType);
        return this;
    }

    /// <summary>
    /// 指定されたアセンブリをもとに自動登録を行うハンドラを追加します。
    /// </summary>
    /// <param name="handler">自動登録を行うハンドラ</param>
    /// <returns><see cref="RequestAwareBehaviorBuilder"/></returns>
    public RequestAwareBehaviorBuilder AddAutoRegistrationHandler(Action<IServiceCollection, Assembly[]> handler)
    {
        _autoRegistrationHandlers.Add(handler);
        return this;
    }

    internal void Build()
    {
        foreach (var openBehaviorType in _openBehaviorTypes.Distinct())
        {
            Services.TryAddTransient(openBehaviorType);
        }
        var assemblies = _assemblies.Distinct().ToArray();
        foreach (var handler in _autoRegistrationHandlers)
        {
            handler(Services, assemblies);
        }
    }

    /// <summary>
    /// 自動登録を行うためのアセンブリを追加します。
    /// </summary>
    /// <param name="assemblies">自動登録を行うためのアセンブリ</param>
    /// <returns><see cref="RequestAwareBehaviorBuilder"/></returns>
    public RequestAwareBehaviorBuilder AddAssembliesForAutoRegister(params Assembly[] assemblies)
    {
        _assemblies.AddRange(assemblies);
        return this;
    }
}
