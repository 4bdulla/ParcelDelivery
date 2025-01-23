using MassTransit;

using Microsoft.EntityFrameworkCore;

using ParcelApi.Data;
using ParcelApi.Data.Abstraction;
using ParcelApi.Services;

using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((_, services, configuration) => configuration
    .MinimumLevel.Verbose()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("MassTransit", LogEventLevel.Warning)
    .WriteTo.Console(new RenderedCompactJsonFormatter())
    .ReadFrom.Services(services));

builder.Services.AddDbContext<IParcelDbContext, ParcelDbContext>(optionsBuilder =>
    optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddAutoMapper(expression => expression.AddMaps(typeof(Program).Assembly));

builder.Services.AddMassTransit(massTransitConfig =>
{
    massTransitConfig.AddConsumer<ParcelApi.Services.ParcelApi>()
        .Endpoint(configurator =>
        {
            configurator.Name = "parcel-api";
        });

    massTransitConfig.UsingRabbitMq((context, rabbitConfig) =>
    {
        rabbitConfig.Host(
            "localhost",
            5672,
            "/",
            configurator =>
            {
                configurator.Username("guest");
                configurator.Password("guest");
            });

        rabbitConfig.ConfigureEndpoints(context);
    });
});


WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (IServiceScope scope = app.Services.CreateScope())
    {
        ParcelDbContext dbContext = scope.ServiceProvider.GetRequiredService<ParcelDbContext>();

        bool created = await dbContext.Database.EnsureCreatedAsync();

        Log.Information("database created: {Created}", created);
    }
}

app.Run();