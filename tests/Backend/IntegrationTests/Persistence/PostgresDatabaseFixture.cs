using Microsoft.EntityFrameworkCore;
using Infrastructure.persistence;
using Testcontainers.PostgreSql;

namespace Backend.IntegrationTests.Persistence;

public sealed class PostgresDatabaseFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("portfolio_tests")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public PortfolioDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PortfolioDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .Options;

        return new PortfolioDbContext(options);
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}
