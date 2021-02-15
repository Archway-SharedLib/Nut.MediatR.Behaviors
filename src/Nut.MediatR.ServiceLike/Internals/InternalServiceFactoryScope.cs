using System;
using MediatR;

namespace Nut.MediatR.ServiceLike.Internals
{
    internal class InternalServiceFactoryScope: IServiceFactoryScope
    {
        public InternalServiceFactoryScope(ServiceFactory serviceFactory)
        {
            this.ServiceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
        }

        public void Dispose()
        {
        }

        public ServiceFactory ServiceFactory { get; }
    }
}