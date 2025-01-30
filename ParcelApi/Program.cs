using Core.Common;

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

try
{
    app.Run();
    app.ReportServiceUp();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception: {Message}", ex.Message);
}
finally
{
    Log.CloseAndFlush();
    app.ReportServiceDown();
}