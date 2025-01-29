﻿using ConfigurationSubstitution;

using Core.Common.Options;

using MassTransit;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;


namespace Core.Common;

public static class Extensions
{
    public static void ConfigureConfiguration(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddEnvironmentVariables();
        builder.Configuration.EnableSubstitutionsWithDelimitedFallbackDefaults("{", "}", ":");
    }

    public static void AddSerilogConfiguration(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((_, services, configuration) => configuration
            .MinimumLevel.Verbose()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("MassTransit", LogEventLevel.Warning)
            .WriteTo.Console(new RenderedCompactJsonFormatter())
            .ReadFrom.Services(services));
    }

    public static void AddSqlDbContext<TDbContextInterface, TDbContextImplementation>(this WebApplicationBuilder builder)
    where TDbContextImplementation : DbContext, TDbContextInterface
    {
        builder.Services.AddDbContext<TDbContextInterface, TDbContextImplementation>(optionsBuilder =>
            optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
    }

    public static void AddMassTransitConfiguration(
        this WebApplicationBuilder builder,
        Action<IBusRegistrationConfigurator> configureAction = null)
    {
        RabbitOptions rabbitOptions = builder.Configuration.GetSection(nameof(RabbitOptions)).Get<RabbitOptions>();

        if (rabbitOptions is null) throw new InvalidOperationException("RabbitOptions is not configured!");

        builder.Services.AddMassTransit(massTransitConfig =>
        {
            configureAction?.Invoke(massTransitConfig);

            massTransitConfig.UsingRabbitMq((context, rabbitConfig) =>
            {
                rabbitConfig.Host(
                    rabbitOptions.Host,
                    rabbitOptions.Port,
                    "/",
                    configurator =>
                    {
                        configurator.Username(rabbitOptions.Username);
                        configurator.Password(rabbitOptions.Password);
                    });

                rabbitConfig.ConfigureEndpoints(context);
            });
        });
    }

    public static AuthOptions ConfigureAuthOptions(this WebApplicationBuilder builder)
    {
        AuthOptions authOptions = builder.Configuration.GetSection(nameof(AuthOptions)).Get<AuthOptions>() ?? AuthOptions.Default;

        builder.Services.AddSingleton(Microsoft.Extensions.Options.Options.Create(authOptions));

        return authOptions;
    }

    public static void UseDbInDevelopment<TDbContext>(this WebApplication app)
    where TDbContext : DbContext
    {
        if (!app.Environment.IsDevelopment())
            return;

        using IServiceScope scope = app.Services.CreateScope();

        TDbContext dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

        bool created = dbContext.Database.EnsureCreated();

        Log.Information("database created: {Created}", created);
    }
}