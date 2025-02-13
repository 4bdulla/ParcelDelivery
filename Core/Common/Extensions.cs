using ConfigurationSubstitution;

using Core.Common.Filters;
using Core.Common.Monitoring;
using Core.Common.Options;

using FluentValidation;

using MassTransit;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

using Prometheus;

using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;


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
            .MinimumLevel.Override("Core.Common.Filters", LogEventLevel.Warning)
            .MinimumLevel.Override("Serilog", LogEventLevel.Warning)
            .WriteTo.Console(new RenderedCompactJsonFormatter())
            .ReadFrom.Services(services));
    }

    public static void AddMonitoring(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<MetricReporter>();
        builder.Services.AddHealthChecks().ForwardToPrometheus(new PrometheusHealthCheckPublisherOptions());
    }

    public static void AddValidators(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<Constants>();
        builder.Services.AddFluentValidationAutoValidation();
    }

    public static void AddSqlDbContext<TDbContextInterface, TDbContextImplementation>(
        this WebApplicationBuilder builder)
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
                rabbitConfig.UseFilters();

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
        AuthOptions options = builder.Configuration.GetSection(nameof(AuthOptions)).Get<AuthOptions>();

        if (!builder.Environment.IsDevelopment() && options is null)
            throw new InvalidOperationException($"{nameof(AuthOptions)} is not configured!");

        AuthOptions authOptions = options ?? AuthOptions.Default;

        builder.Services.AddSingleton(Microsoft.Extensions.Options.Options.Create(authOptions));

        return authOptions;
    }


    public static void UseSerilogMiddleware(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.GetLevel += (context, elapsed, ex) =>
                ex is not null || context?.Response.StatusCode >= 500
                    ? LogEventLevel.Error
                    : context?.Response.StatusCode >= 400
                        ? LogEventLevel.Warning
                        : elapsed > TimeSpan.FromSeconds(2).TotalMilliseconds
                            ? LogEventLevel.Warning
                            : LogEventLevel.Verbose;
        });
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

    public static void UseMonitoring(this WebApplication app)
    {
        app.UseHealthChecks("/health");

        app.UseHttpMetrics(o => o.ReduceStatusCodeCardinality());

        app.MapMetrics();
    }


    private static void UseFilters<T>(this IPipeConfigurator<T> configurator)
    where T : class, PipeContext
    {
        configurator.AddPipeSpecification(new LoggingSpecification<T>());
        configurator.AddPipeSpecification(new MonitoringSpecification<T>(new MetricReporter()));
    }
}