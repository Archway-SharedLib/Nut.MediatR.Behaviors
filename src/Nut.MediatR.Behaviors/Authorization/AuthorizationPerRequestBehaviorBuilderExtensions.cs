namespace Nut.MediatR;

public static class AuthorizationPerRequestBehaviorBuilderExtensions
{
    public static PerRequsetBehaviorBuilder AddAuthorization(this PerRequsetBehaviorBuilder builder)
    {
        builder.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
        builder.AddAutoRegistrationHandler((services, assemblies) =>
        {
            services.TryAddTransientGenericInterfaceTypeFromAssemblies(assemblies, typeof(IAuthorizer<>));
        });
        return builder;
    }
}
