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
                .Where(type => MediatorRequest.CanServicalize(type));
            foreach(var request in requests)
            {
                requestRegistry.Add(request, true, filterTypes);
            }

            if (!(services.LastOrDefault(s => s.ServiceType == typeof(EventRegistry))?
                .ImplementationInstance is EventRegistry eventRegistry))
            {
                eventRegistry = new EventRegistry();
            }
            services.TryAddSingleton(eventRegistry);

            var events = assembly.GetTypes()
                .Where(type => MediatorEvent.CanEventalize(type));
            foreach (var ev in events)
            {
                eventRegistry.Add(ev, true);
            }

            services.TryAddTransient(typeof(IMediatorClient), provider =>
            {
                var requestRegistry = provider.GetService<RequestRegistry>();
                var eventRegistry = provider.GetService<EventRegistry>();
                var serviceFactory = provider.GetService<ServiceFactory>();
                return new DefaultMediatorClient(requestRegistry, eventRegistry, serviceFactory);
            });

            return services;
        }
    }
}
