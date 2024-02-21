using System;
using MediatR.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Nut.MediatR;

public static class ServiceCollectionExtesions
{
    public static IServiceCollection AddMediatRPerRequestBehavior(this IServiceCollection service, Action<PerRequsetBehaviorBuilder> optionBuilder)
    {
        var mediatrConfiguration = new MediatRServiceConfiguration();
        mediatrConfiguration.AddOpenBehavior(typeof(PerRequestBehavior<,>));
        ServiceRegistrar.AddRequiredServices(service, mediatrConfiguration);
        var builder = new PerRequsetBehaviorBuilder(service);
        optionBuilder(builder);
        builder.Build();
        return service;
    }
}
