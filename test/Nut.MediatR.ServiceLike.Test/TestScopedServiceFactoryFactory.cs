using System;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Nut.MediatR.ServiceLike.Test;

internal class InternalScopedServiceFactoryFactory : IScopedServiceFactoryFactory
{
    private readonly IServiceProvider _serviceProvider;

    public InternalScopedServiceFactoryFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public IScoepedServiceFactory Create() => new InternalScopedServiceFactory(_serviceProvider);
}

internal class InternalScopedServiceFactory : IScoepedServiceFactory
{
    public InternalScopedServiceFactory(IServiceProvider serviceProvider)
    {
        Instance = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public void Dispose()
    {
    }

    public IServiceProvider Instance { get; }
}

public class TestScopedServiceFactoryFactory: IScopedServiceFactoryFactory
{
    private readonly ServiceProvider _serviceProvider;

    public TestScopedServiceFactoryFactory(ServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IScoepedServiceFactory Create()
    {
        return new TestScopedServiceFactory(_serviceProvider);
    }
}

public class TestScopedServiceFactory: IScoepedServiceFactory
{
    private readonly IServiceScope _scope;
    public TestScopedServiceFactory(ServiceProvider provider)
    {
        _scope = provider.CreateScope();
        Instance = _scope.ServiceProvider;
    }
    public IServiceProvider Instance { get; }

    public void Dispose()
    {
        if(_scope is not null) _scope.Dispose();
    }
}
