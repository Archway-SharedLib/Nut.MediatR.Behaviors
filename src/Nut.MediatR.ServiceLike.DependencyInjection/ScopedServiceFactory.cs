using System;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Nut.MediatR.ServiceLike.DependencyInjection;

internal class ScopedServiceFactory : IScoepedServiceFactory
{
    private readonly IServiceScope _scope;

    public ScopedServiceFactory(IServiceScopeFactory scopeFactory)
    {
        if (scopeFactory is null) throw new ArgumentNullException(nameof(scopeFactory));
        _scope = scopeFactory.CreateScope();
        Instance = _scope.ServiceProvider.GetService;
    }

    public void Dispose()
    {
        if (_scope is not null) _scope.Dispose();
    }

    public ServiceFactory Instance { get; }
}
