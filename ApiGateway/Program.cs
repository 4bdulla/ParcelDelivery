using System.Reflection;

using ApiGateway;

using Core.Common;
using Core.Queries;

using MassTransit;

using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;


var builder = WebApplication.CreateBuilder(args);


builder.Host.UseSerilog((_, services, configuration) => configuration
    .MinimumLevel.Verbose()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("MassTransit", LogEventLevel.Warning)
    .WriteTo.Console(new RenderedCompactJsonFormatter())
    .ReadFrom.Services(services));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var coreDocsFilename = $"{typeof(ParcelNotFoundResponse).Assembly.GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, coreDocsFilename));
});

builder.Services.AddMassTransit(massTransitConfig =>
{
    massTransitConfig.AddRequestClient<GetParcelRequest>(new Uri("exchange:parcel-api"));

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

var app = builder.Build();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapEndpoints();

app.Run();