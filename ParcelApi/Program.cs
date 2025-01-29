using Core.Common;

using ParcelApi.Data;
using ParcelApi.Data.Abstraction;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.ConfigureConfiguration();
builder.AddSerilogConfiguration();
builder.AddSqlDbContext<IParcelDbContext, ParcelDbContext>();
builder.Services.AddAutoMapper(expression => expression.AddMaps(typeof(Program).Assembly));

builder.AddMassTransitConfiguration(configurator =>
    configurator.AddConsumer<ParcelApi.Services.ParcelApi>().Endpoint(c => c.Name = nameof(ParcelApi)));

WebApplication app = builder.Build();

app.UseDbInDevelopment<ParcelDbContext>();

app.Run();