using System;
using MediatR;

namespace Nut.MediatR.ServiceLike.Internals;

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
