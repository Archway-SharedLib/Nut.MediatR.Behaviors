using System.Reflection;
using System;

namespace Nut.MediatR;

/// <summary>
/// <see cref="RequestAwareBehaviorBuilder"/> の拡張メソッドを定義します。
/// </summary>
public static class LoggingRequestAwareBehaviorBuilderExtensions
{
    /// <summary>
    /// ログ出力を行う <see cref="LoggingBehavior{TRequest, TResponse}"/> を追加します。
    /// 利用する <see cref="ILoggingInOutValueCollector{TRequest}"/> は<see cref="RequestAwareBehaviorBuilder"/>で指定されているアセンブリから自動登録されます。
    /// </summary>
    /// <param name="builder"><see cref="RequestAwareBehaviorBuilder"/></param>
    /// <returns>元となった <see cref="RequestAwareBehaviorBuilder"/></returns>
    public static RequestAwareBehaviorBuilder AddLogging(this RequestAwareBehaviorBuilder builder)
    {
        builder.AddOpenBehavior(typeof(LoggingBehavior<,>));
        builder.AddAutoRegistrationHandler((services, assemblies) =>
        {
            services.TryAddTransientGenericInterfaceTypeFromAssemblies(assemblies, typeof(ILoggingInOutValueCollector<,>));
        });
        return builder;
    }

    /// <summary>
    /// ログ出力を行う <see cref="LoggingBehavior{TRequest, TResponse}"/> を追加します。
    /// 利用する <see cref="ILoggingInOutValueCollector{TRequest}"/> は指定された <paramref name="assembliesForAutoRegister"/> から自動登録されます。
    /// </summary>
    /// <param name="builder"><see cref="RequestAwareBehaviorBuilder"/></param>
    /// <param name="assembliesForAutoRegister"><see cref="ILoggingInOutValueCollector{TRequest}"/>を含む <see cref="Assembly"/></param>
    /// <returns>元となった <see cref="RequestAwareBehaviorBuilder"/></returns>
    public static RequestAwareBehaviorBuilder AddLogging(this RequestAwareBehaviorBuilder builder, params Assembly[] assembliesForAutoRegister)
    {
        builder.AddOpenBehavior(typeof(LoggingBehavior<,>));
        var assemblies = assembliesForAutoRegister ?? Array.Empty<Assembly>();
        builder.AddAutoRegistrationHandler((services, _) =>
        {
            services.TryAddTransientGenericInterfaceTypeFromAssemblies(assemblies, typeof(ILoggingInOutValueCollector<>));
        });
        return builder;
    }
}
