using System;
using MediatR;

namespace Nut.MediatR.ServiceLike.Internals
{
    internal class InternalScopedServiceFactoryFactory: IScopedServiceFactoryFactory
    {
        private readonly ServiceFactory serviceFactory;

        public InternalScopedServiceFactoryFactory(ServiceFactory serviceFactory)
        {
            this.serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
        }

        public IServiceFactoryScope Create() => new InternalServiceFactoryScope(serviceFactory);
    }
}