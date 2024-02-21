namespace Nut.MediatR;

public static class LoggingPerRequestBehaviorBuilderExtensions
{
    public static PerRequsetBehaviorBuilder AddLogging(this PerRequsetBehaviorBuilder builder)
    {
        builder.AddOpenBehavior(typeof(LoggingBehavior<,>));
        builder.AddAutoRegistrationHandler((services, assemblies) =>
        {
            services.TryAddTransientGenericInterfaceTypeFromAssemblies(assemblies, typeof(ILoggingInOutValueCollector<,>));
        });
        return builder;
    }
}
