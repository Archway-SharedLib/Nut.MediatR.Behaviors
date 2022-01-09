using System;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Nut.MediatR.ServiceLike.DependencyInjection;

internal class ScopedServiceFactory : IScoepedServiceFactory
{
    private readonly IServiceScope scope;
    public ScopedServiceFactory(IServiceScopeFactory scopeFactory)
    {
        if (scopeFactory is null) throw new ArgumentNullException(nameof(scopeFactory));
        scope = scopeFactory.CreateScope();
        Instance = scope.ServiceProvider.GetService;
    }

    public void Dispose()
    {
        if (!(scope is null)) scope.Dispose();
    }

    public ServiceFactory Instance { get; }
}
