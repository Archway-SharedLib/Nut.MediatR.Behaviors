using System;
using Microsoft.Extensions.DependencyInjection;

namespace Nut.MediatR.ServiceLike.DependencyInjection;

internal class ScopedServiceFactoryFactory : IScopedServiceFactoryFactory
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ScopedServiceFactoryFactory(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
    }
    public IScoepedServiceFactory Create()
    {
        return new ScopedServiceFactory(_scopeFactory);
    }
}
