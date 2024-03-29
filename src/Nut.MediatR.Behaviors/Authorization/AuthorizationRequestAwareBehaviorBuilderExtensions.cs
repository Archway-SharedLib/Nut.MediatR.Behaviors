using System;
using System.Reflection;
using Nut.MediatR.Internals;

namespace Nut.MediatR;

/// <summary>
/// <see cref="RequestAwareBehaviorBuilder"/> の拡張メソッドを定義します。
/// </summary>
public static class AuthorizationRequestAwareBehaviorBuilderExtensions
{
    /// <summary>
    /// 認可を行う <see cref="AuthorizationBehavior{TRequest, TResponse}"/> を追加します。
    /// 利用する <see cref="IAuthorizer{TRequest}"/> は<see cref="RequestAwareBehaviorBuilder"/>で指定されているアセンブリから自動登録されます。
    /// </summary>
    /// <param name="builder"><see cref="RequestAwareBehaviorBuilder"/></param>
    /// <returns>元となった <see cref="RequestAwareBehaviorBuilder"/></returns>
    public static RequestAwareBehaviorBuilder AddAuthorization(this RequestAwareBehaviorBuilder builder)
    {
        builder.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
        builder.AddAutoRegistrationHandler((services, assemblies) =>
        {
            services.TryAddTransientGenericInterfaceTypeFromAssemblies(assemblies, typeof(IAuthorizer<>));
        });
        return builder;
    }

    /// <summary>
    /// 認可を行う <see cref="AuthorizationBehavior{TRequest, TResponse}"/> を追加します。
    /// 利用する <see cref="IAuthorizer{TRequest}"/> は指定された <paramref name="assembliesForAutoRegister"/> から自動登録されます。
    /// </summary>
    /// <param name="builder"><see cref="RequestAwareBehaviorBuilder"/></param>
    /// <param name="assembliesForAutoRegister"><see cref="IAuthorizer{TRequest}"/>を含む <see cref="Assembly"/></param>
    /// <returns>元となった <see cref="RequestAwareBehaviorBuilder"/></returns>
    public static RequestAwareBehaviorBuilder AddAuthorization(this RequestAwareBehaviorBuilder builder, params Assembly[] assembliesForAutoRegister)
    {
        builder.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
        var assemblies = assembliesForAutoRegister ?? Array.Empty<Assembly>();
        builder.AddAutoRegistrationHandler((services, _) =>
        {
            services.TryAddTransientGenericInterfaceTypeFromAssemblies(assemblies, typeof(IAuthorizer<>));
        });
        return builder;
    }
}
