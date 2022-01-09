using System;
using Microsoft.Extensions.DependencyInjection;

namespace Nut.MediatR.ServiceLike.DependencyInjection;

internal class ScopedServiceFactoryFactory : IScopedServiceFactoryFactory
{
    private readonly IServiceScopeFactory scopeFactory;

    public ScopedServiceFactoryFactory(IServiceScopeFactory scopeFactory)
    {
        this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
    }
    public IScoepedServiceFactory Create()
    {
        return new ScopedServiceFactory(scopeFactory);
    }
}
