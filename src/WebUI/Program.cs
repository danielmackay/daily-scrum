using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System.Reflection;
using WebUI.Common.Identity;
using WebUI.Features.DailyScrum.UseCases.CreateDailyScrumCommand;
using WebUI.Features.DailyScrum.UseCases.CreateDailyScrumCommand.Infrastructure;
using WebUI.Host;

var appAssembly = Assembly.GetExecutingAssembly();
var builder = WebApplication.CreateBuilder(args);

var initialScopes = builder.Configuration.GetSection("DownstreamApi:Scopes").Get<string[]>();

builder.Services
    .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
    .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
    .AddMicrosoftGraph(builder.Configuration.GetSection("DownstreamApi"))
    // TODO: Consider switching to session cache to make development easier
    // .AddDistributedTokenCaches()
    // .AddSessionTokenCaches();
    .AddInMemoryTokenCaches();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddDistributedMemoryCache();

builder.Services
    .AddRazorPages()
    .AddMicrosoftIdentityUI();

// App Services
builder.Services.ConfigureFeatures(builder.Configuration, appAssembly);
builder.Services.AddMediatR();
// builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set the session timeout
    options.Cookie.HttpOnly = true; // Make the session cookie HTTP only
    options.Cookie.IsEssential = true; // Make the session cookie essential
});

builder.Services.AddScoped<IDailyScrumRepository, SessionDailyScrumRepository>();
builder.Services.AddScoped<GraphServiceClientFactory>();
builder.Services.AddScoped<ICurrentUserService, OAuthCurrentUserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

// app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapStaticAssets();
app.MapRazorPages()
    .WithStaticAssets();
app.MapControllers();

app.Run();
