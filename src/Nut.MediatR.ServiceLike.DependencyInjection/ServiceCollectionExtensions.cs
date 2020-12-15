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
                .ImplementationInstance is RequestRegistry registry))
            {
                registry = new RequestRegistry();
            }
            services.TryAddSingleton(registry);

            var requests = assembly.GetTypes()
                .Where(type => MediatorRequest.CanServicalize(type));
            foreach(var request in requests)
            {
                registry.Add(request, true, filterTypes);
            }

            services.TryAddTransient<IMediatorClient, DefaultMediatorClient>();

            return services;
        }
    }
}
