using WebUI.Common.Features;
using WebUI.Features.DailyScrum.Infrastructure;

namespace WebUI.Features.Timesheet;

public sealed class TimesheetFeature : IFeature
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        //services.AddOptionsWithValidation<MicrosoftGraphOptions>(MicrosoftGraphOptions.Section);
        //services.AddScoped<IGraphService, MockGraphService>();
        //services.AddScoped<IGraphService, GraphService>();
    }
}
