using System.Data.Common;

using AutoMapper;

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


namespace Test.Utility;

public class ParcelDeliveryWebApplicationFactory<TProgram, TDbContextInterface, TDbContext>
    : WebApplicationFactory<TProgram>
where TProgram : class
where TDbContext : DbContext, TDbContextInterface
{
    public ITestHarness TestHarness => base.Services.GetTestHarness();
    public TDbContext DbContext => this.GetRequiredService<TDbContext>();


    public void EnsureDbCreated()
    {
        TDbContext db = this.GetRequiredService<TDbContext>();

        db.Database.EnsureCreated();
    }

    public void EnsureDbDeleted()
    {
        TDbContext db = this.GetRequiredService<TDbContext>();

        db.Database.EnsureDeleted();
    }

    public async Task SeedDataAsync(IEnumerable<object> data)
    {
        TDbContext db = this.GetRequiredService<TDbContext>();

        await db.AddRangeAsync(data, CancellationToken.None);
        await db.SaveChangesAsync();
    }

    public async Task<T> GetData<T>(object key)
    where T : class => await this.GetRequiredService<TDbContext>().FindAsync<T>(key);


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

    public TService GetRequiredService<TService>()
    {
        IServiceScope scope = base.Services.CreateScope();

        return scope.ServiceProvider.GetRequiredService<TService>();
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
            services.RemoveAll<TDbContextInterface>();
            services.RemoveAll<IDbContextOptionsConfiguration<TDbContext>>();
            services.RemoveAll<DbContextOptions<TDbContext>>();

            services.AddSingleton<DbConnection>(_ =>
                new SqliteConnection(context.Configuration.GetConnectionString("Default")));

            services.AddDbContext<TDbContextInterface, TDbContext>((s, options) =>
                options.UseSqlite(s.GetRequiredService<DbConnection>()));
        });
    }
}