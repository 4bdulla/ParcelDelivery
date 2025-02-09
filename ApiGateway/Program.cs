using System.Reflection;

using ApiGateway;

using Core.Common;
using Core.Common.ErrorResponses;
using Core.Common.Monitoring;
using Core.Common.Options;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

using NetDevPack.Security.JwtExtensions;

using Serilog;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.ConfigureConfiguration();
builder.AddSerilogConfiguration();
builder.AddMonitoring();
builder.AddValidators();

AuthOptions authOptions = builder.ConfigureAuthOptions();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var coreDocsFilename = $"{typeof(ParcelNotFoundResponse).Assembly.GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, coreDocsFilename));

    options.AddSecurityDefinition("Bearer",
        new()
        {
            Description = "Bearer {token}",
            Name = "Authorization",
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey
        });

    options.AddSecurityRequirement(new()
    {
        { new() { Reference = new() { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, [] }
    });
});

builder.AddMassTransitConfiguration();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;

        x.SetJwksOptions(new(
            authOptions.AuthServerAddress,
            authOptions.JwtIssuer,
            authOptions.TokenLifetime,
            authOptions.JwtAudience));
    });

builder.Services.AddAuthorization();

WebApplication app = builder.Build();

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
app.UseMonitoring();
app.MapEndpoints();

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