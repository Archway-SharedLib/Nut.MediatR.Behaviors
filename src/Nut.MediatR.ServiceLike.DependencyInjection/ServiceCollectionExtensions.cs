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
            if (!(services.LastOrDefault(s => s.ServiceType == typeof(RequestRegistry))?
                .ImplementationInstance is RequestRegistry requestRegistry))
            {
                requestRegistry = new RequestRegistry();
            }
            services.TryAddSingleton(requestRegistry);

            var requests = assembly.GetTypes()
                .Where(MediatorRequest.CanServicalize);
            foreach(var request in requests)
            {
                requestRegistry.Add(request, true, filterTypes);
            }

            if (!(services.LastOrDefault(s => s.ServiceType == typeof(NotificationRegistry))?
                .ImplementationInstance is NotificationRegistry notificationRegistry))
            {
                notificationRegistry = new NotificationRegistry();
            }
            services.TryAddSingleton(notificationRegistry);

            var notifications = assembly.GetTypes()
                .Where(MediatorNotification.CanEventalize);
            foreach (var notification in notifications)
            {
                notificationRegistry.Add(notification);
            }
            
            services.TryAddTransient(typeof(IMediatorClient), provider =>
            {
                var reqRegistry = provider.GetService<RequestRegistry>();
                var notiRegistry = provider.GetService<NotificationRegistry>();
                var serviceFactory = provider.GetService<ServiceFactory>();
                var scopedServiceFactoryFactory = new ScopedServiceFactoryFactory(provider);
                
                // TODO: create logger
                return new DefaultMediatorClient(
                    reqRegistry, 
                    notiRegistry, 
                    serviceFactory,
                    scopedServiceFactoryFactory, null!);
            });

            return services;
        }
    }
}
