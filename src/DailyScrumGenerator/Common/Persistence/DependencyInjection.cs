﻿using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DailyScrumGenerator.Common.Persistence;

public static class DependencyInjection
{
    public static void AddEfCore(this IServiceCollection services)
    {
        services.AddScoped<ISaveChangesInterceptor, EventPublisher>();

        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.UseInMemoryDatabase("InMemoryDbForTesting");

            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
        });
    }
}