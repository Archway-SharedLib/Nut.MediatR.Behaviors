using System;

namespace Nut.MediatR.ServiceLike.DependencyInjection
{
    internal class ScopedServiceFactoryFactory: IScopedServiceFactoryFactory
    {
        private readonly IServiceProvider provider;

        public ScopedServiceFactoryFactory(IServiceProvider provider)
        {
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }
        public IServiceFactoryScope Create()
        {
            return new ServiceFactoryScope(provider);
        }
    }
}