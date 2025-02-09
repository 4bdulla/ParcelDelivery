using Core.Common;
using Core.Common.Monitoring;

using ParcelApi.Data;
using ParcelApi.Data.Abstraction;

using Serilog;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.ConfigureConfiguration();
builder.AddSerilogConfiguration();
builder.AddMonitoring();

builder.AddSqlDbContext<IParcelDbContext, ParcelDbContext>();

builder.Services.AddAutoMapper(expression => expression.AddMaps(typeof(Program).Assembly));

builder.AddMassTransitConfiguration(configurator =>
    configurator.AddConsumer<ParcelApi.Services.ParcelApi>().Endpoint(c => c.Name = nameof(ParcelApi)));

WebApplication app = builder.Build();

app.UseMonitoring();
app.UseDbInDevelopment<ParcelDbContext>();

MetricReporter metricReporter = app.Services.GetRequiredService<MetricReporter>();

string applicationName = app.Environment.ApplicationName;

try
{
    app.Run();
    metricReporter.ServiceUp(applicationName);
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception: {Message}", ex.Message);
}
finally
{
    Log.CloseAndFlush();
    metricReporter.ServiceDown(applicationName);
}

public partial class Program;