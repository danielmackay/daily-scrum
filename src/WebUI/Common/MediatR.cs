using WebUI.Common.Behaviors;

namespace WebUI.Host;

public static class MediatR
{
    public static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        var applicationAssembly = typeof(MediatR).Assembly;

        //services.AddValidatorsFromAssembly(applicationAssembly);

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(applicationAssembly);
            config.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
            //config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            config.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
        });

        return services;
    }
}
