using System;
using MediatR.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Nut.MediatR;

/// <summary>
/// <see cref="IServiceCollection"/> の拡張メソッドを提供します。
/// </summary>
public static class ServiceCollectionExtesions
{
    /// <summary>
    /// <see cref="RequestAwareBehavior{TRequest, TResponse}"/> を利用するための構成を定義します。
    /// </summary>
    /// <param name="service">元となる <see cref="IServiceCollection"/></param>
    /// <param name="optionBuilder">オプション構成を指定するビルダ</param>
    /// <returns>元となった <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddMediatRRequestAwareBehavior(this IServiceCollection service, Action<RequestAwareBehaviorBuilder> optionBuilder)
    {
        var mediatrConfiguration = new MediatRServiceConfiguration();
        mediatrConfiguration.AddOpenBehavior(typeof(RequestAwareBehavior<,>));
        ServiceRegistrar.AddRequiredServices(service, mediatrConfiguration);
        var builder = new RequestAwareBehaviorBuilder(service);
        optionBuilder(builder);
        builder.Build();
        return service;
    }
}
