using Application.Common.Interfaces;
using Infrastructure.persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<PortfolioDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PortfolioDb")));

        services.AddScoped<IProjectRepository, ProjectRepository>();

        return services;
    }
}