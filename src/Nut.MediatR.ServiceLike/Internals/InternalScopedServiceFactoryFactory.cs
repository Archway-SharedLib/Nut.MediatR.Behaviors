using System;
using MediatR;

namespace Nut.MediatR.ServiceLike.Internals;

internal class InternalScopedServiceFactoryFactory : IScopedServiceFactoryFactory
{
    private readonly ServiceFactory _serviceFactory;

    public InternalScopedServiceFactoryFactory(ServiceFactory serviceFactory)
    {
        _serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
    }

    public IScoepedServiceFactory Create() => new InternalScopedServiceFactory(_serviceFactory);
}
