using WebUI.Common.Features;
using WebUI.Common.Services;
using WebUI.Features.DailyScrum.Infrastructure;

namespace WebUI.Features.DailyScrum;

public sealed class DailyScrumFeature : IFeature
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        //services.AddOptionsWithValidation<MicrosoftGraphOptions>(MicrosoftGraphOptions.Section);
        //services.AddScoped<IGraphService, MockGraphService>();
        services.AddScoped<IGraphService, GraphService>();
        services.AddScoped<TimeProvider, SydneyTimeProvider>();
    }
}
