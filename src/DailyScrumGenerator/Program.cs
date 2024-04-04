using System.Reflection;
using DailyScrumGenerator;
using DailyScrumGenerator.Host;

var appAssembly = Assembly.GetExecutingAssembly();
var builder = WebApplication.CreateBuilder(args);

// Common
builder.Services.AddEfCore();

// Host
builder.Services.AddRazorPages();
builder.Services.AddHandlers();
builder.Services.AddBehaviors();
// builder.Services.AddSwaggerGen( options =>
// {
//     options.CustomSchemaIds(x => x.FullName?.Replace("+", ".", StringComparison.Ordinal));
// });

builder.Services.AddMediatR(configure => configure.RegisterServicesFromAssemblyContaining<Program>());
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<ExceptionHandler.KnownExceptionsHandler>();

builder.Services.ConfigureFeatures(builder.Configuration, appAssembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // app.UseSwagger();
    // app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

app.UseProductionExceptionHandler();

// No endpoints for now
//app.RegisterEndpoints(appAssembly);



app.Run();
