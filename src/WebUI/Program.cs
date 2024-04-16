using Microsoft.AspNetCore.HttpOverrides;
using System.Reflection;
using WebUI.Common;
using WebUI.Components;
using WebUI.Host;

var appAssembly = Assembly.GetExecutingAssembly();
var builder = WebApplication.CreateBuilder(args);

// Framework Services
builder.Services.AddRazorComponents();

// App Services
builder.Services.ConfigureFeatures(builder.Configuration, appAssembly);
builder.Services.AddMediatR();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddCors();

var app = builder.Build();

app.UseCors();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();


}

//app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>();


app.Run();
