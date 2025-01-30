using AuthApi.Data;

using Core.Common;
using Core.Common.Options;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;

using Serilog;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.ConfigureConfiguration();
builder.AddSerilogConfiguration();
builder.AddMonitoring();

AuthOptions authOptions = builder.ConfigureAuthOptions();

builder.AddSqlDbContext<IAuthDbContext, AuthDbContext>();
builder.Services.AddJwksManager().UseJwtValidation();
builder.Services.AddMemoryCache();

builder.AddMassTransitConfiguration(configurator =>
    configurator.AddConsumer<AuthApi.Services.AuthApi>().Endpoint(c => c.Name = nameof(AuthApi)));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = authOptions.JwtIssuer,
            ValidAudience = authOptions.JwtIssuer
        };
    });

builder.Services
    .AddIdentity<DomainUser, IdentityRole<int>>(options =>
    {
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization();

WebApplication app = builder.Build();

app.UseJwksDiscovery();
app.UseAuthentication();
app.UseAuthorization();
app.UseMonitoring();
app.UseDbInDevelopment<AuthDbContext>();

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