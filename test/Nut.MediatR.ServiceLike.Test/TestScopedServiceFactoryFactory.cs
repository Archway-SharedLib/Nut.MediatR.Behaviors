using System;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Nut.MediatR.ServiceLike.Test;

internal class InternalScopedServiceFactoryFactory : IScopedServiceFactoryFactory
{
    private readonly ServiceFactory _serviceFactory;

    public InternalScopedServiceFactoryFactory(ServiceFactory serviceFactory)
    {
        _serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
    }

    public IScoepedServiceFactory Create() => new InternalScopedServiceFactory(_serviceFactory);
}

internal class InternalScopedServiceFactory : IScoepedServiceFactory
{
    public InternalScopedServiceFactory(ServiceFactory serviceFactory)
    {
        Instance = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
    }

    public void Dispose()
    {
    }

    public ServiceFactory Instance { get; }
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
        Instance = _scope.ServiceProvider.GetService;
    }
    public ServiceFactory Instance { get; }

    public void Dispose()
    {
        if(_scope is not null) _scope.Dispose();
    }
}
