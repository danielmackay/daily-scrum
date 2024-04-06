namespace WebUI.Common.Configuration;

public static class ServiceCollectionExt
{
    public static IServiceCollection AddOptionsWithValidation<TOptions>(this IServiceCollection services,
        string sectionName)
        where TOptions : class
    {
        services
            .AddOptionsWithValidateOnStart<TOptions>()
            .BindConfiguration(sectionName)
            .ValidateDataAnnotations()
            ;

        return services;
    }
}
