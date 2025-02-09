using System.Data.Common;
using System.Net.Http.Json;

using AutoMapper;

using Castle.DynamicProxy.Generators;

using FluentAssertions;

using MassTransit;
using MassTransit.Testing;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using ParcelApi.Data;
using ParcelApi.Data.Abstraction;


namespace ParcelApi.IntegrationTests;

public class ParcelApiWebApplicationFactory : WebApplicationFactory<Program>
{
    public ITestHarness TestHarness => base.Services.GetTestHarness();
    public ParcelDbContext DbContext => this.GetRequiredService<ParcelDbContext>();
    public IMapper Mapper => this.GetRequiredService<IMapper>();


    public void EnsureDbCreated()
    {
        ParcelDbContext db = this.GetRequiredService<ParcelDbContext>();

        db.Database.EnsureCreated();
    }

    public void EnsureDbDeleted()
    {
        ParcelDbContext db = this.GetRequiredService<ParcelDbContext>();

        db.Database.EnsureDeleted();
    }

    public async Task SeedDataAsync(IEnumerable<object> data)
    {
        ParcelDbContext db = this.GetRequiredService<ParcelDbContext>();

        await db.AddRangeAsync(data, CancellationToken.None);
        await db.SaveChangesAsync();
    }

    public async Task<T> GetData<T>(object key)
    where T : class => await this.GetRequiredService<ParcelDbContext>().FindAsync<T>(key);


    public async Task AssertResponse<TConsumer, TRequest, TResponse>()
    where TRequest : class
    where TResponse : class
    where TConsumer : class, IConsumer
    {
        bool consumerConsumedRequest = await this.TestHarness.GetConsumerHarness<TConsumer>().Consumed.Any<TRequest>();
        consumerConsumedRequest.Should().BeTrue();

        bool requestConsumed = await this.TestHarness.Consumed.Any<TRequest>();
        requestConsumed.Should().BeTrue();

        bool responseSent = await this.TestHarness.Sent.Any<TResponse>();
        responseSent.Should().BeTrue();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(config =>
        {
            config.Sources.Clear();
            config.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
            config.AddJsonFile("./appsettings.Testing.json");
        });

        builder.ConfigureServices((context, services) =>
        {
            services.AddMassTransitTestHarness();
            services.RemoveAll<IParcelDbContext>();
            services.RemoveAll<IDbContextOptionsConfiguration<ParcelDbContext>>();
            services.RemoveAll<DbContextOptions<ParcelDbContext>>();

            services.AddSingleton<DbConnection>(_ =>
                new SqliteConnection(context.Configuration.GetConnectionString("Default")));

            services.AddDbContext<IParcelDbContext, ParcelDbContext>((s, options) =>
                options.UseSqlite(s.GetRequiredService<DbConnection>()));
        });
    }

    private TService GetRequiredService<TService>()
    {
        IServiceScope scope = base.Services.CreateScope();

        return scope.ServiceProvider.GetRequiredService<TService>();
    }
}