using Application.Portfolio.Projects.CreateProject;
using Application.Portfolio.Projects.UpdateProject;
using Microsoft.Extensions.DependencyInjection;

namespace Application.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<CreateProjectHandler>();
        services.AddScoped<UpdateProjectHandler>();

        return services;
    }
}
