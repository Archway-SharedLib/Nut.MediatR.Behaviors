using System;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Nut.MediatR.ServiceLike.DependencyInjection
{
    internal class ServiceFactoryScope : IServiceFactoryScope
    {
        private readonly IServiceScope scope;
        public ServiceFactoryScope(IServiceProvider provider)
        {
            if (provider is null) throw new ArgumentNullException(nameof(provider));
            scope = provider.CreateScope();
            ServiceFactory = scope.ServiceProvider.GetService;
        }
        
        public void Dispose()
        {
            if (!(scope is null)) scope.Dispose();
        }

        public ServiceFactory ServiceFactory { get; }
    }
}