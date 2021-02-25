using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;
using System.Reflection;

namespace Nut.MediatR.ServiceLike.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMediatRServiceLike(this IServiceCollection services, Assembly assembly, params Type[] filterTypes)
        {
            if (!(services.LastOrDefault(s => s.ServiceType == typeof(ServiceRegistry))?
                .ImplementationInstance is ServiceRegistry serviceRegistry))
            {
                serviceRegistry = new ServiceRegistry();
            }
            services.TryAddSingleton(serviceRegistry);

            var serviceDescriptions = assembly.GetTypes()
                .Where(MediatorServiceDescription.CanServicalize);
            foreach(var serviceDescription in serviceDescriptions)
            {
                serviceRegistry.Add(serviceDescription, true, filterTypes);
            }

            if (!(services.LastOrDefault(s => s.ServiceType == typeof(ListenerRegistry))?
                .ImplementationInstance is ListenerRegistry listenerRegistry))
            {
                listenerRegistry = new ListenerRegistry();
            }
            services.TryAddSingleton(listenerRegistry);

            var listenerDescriptions = assembly.GetTypes()
                .Where(MediatorListenerDescription.CanListenerize);
            foreach (var listenerDescription in listenerDescriptions)
            {
                listenerRegistry.Add(listenerDescription);
            }
            
            services.TryAddTransient(typeof(IMediatorClient), provider =>
            {
                var servRegistry = provider.GetService<ServiceRegistry>();
                var lisRegistry = provider.GetService<ListenerRegistry>();
                var serviceFactory = provider.GetService<ServiceFactory>();
                var scopedServiceFactoryFactory = new ScopedServiceFactoryFactory(provider.GetService<IServiceScopeFactory>());
                var serviceLikeLogger = provider.GetService<IServiceLikeLogger>();
                
                return new DefaultMediatorClient(
                    servRegistry, 
                    lisRegistry, 
                    serviceFactory,
                    scopedServiceFactoryFactory, 
                    serviceLikeLogger);
            });

            return services;
        }
    }
}
