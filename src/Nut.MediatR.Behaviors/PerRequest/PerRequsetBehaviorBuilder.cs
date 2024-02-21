using MediatR;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace Nut.MediatR;

public class PerRequsetBehaviorBuilder
{

    public PerRequsetBehaviorBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; }

    private readonly List<Type> _openBehaviorTypes = new();

    private readonly List<Action<IServiceCollection, Assembly[]>> _autoRegistrationHandlers = new();

    private readonly List<Assembly> _assemblies = new();

    public PerRequsetBehaviorBuilder AddOpenBehavior(Type openBehaviorType)
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

    public PerRequsetBehaviorBuilder AddAutoRegistrationHandler(Action<IServiceCollection, Assembly[]> handler)
    {
        _autoRegistrationHandlers.Add(handler);
        return this;
    }

    internal void Build()
    {
        foreach(var openBehaviorType in _openBehaviorTypes.Distinct())
        {
            Services.TryAddTransient(openBehaviorType);
        }
        var assemblies = _assemblies.Distinct().ToArray();
        foreach(var handler in _autoRegistrationHandlers)
        {
            handler(Services, assemblies);
        }
    }

    public PerRequsetBehaviorBuilder AddAssembliesForAutoRegister(params Assembly[] assemblies)
    {
        _assemblies.AddRange(assemblies);
        return this;
    }
}

