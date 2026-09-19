using Microsoft.AspNetCore.Http;
using Application.Portfolio.Projects.CreateProject;

namespace Api.Projects.CreateProject;

public static class CreateProjectEndpoint
{
    public static IEndpointRouteBuilder MapCreateProjectEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(
            "/api/projects",
            async (
                CreateProjectRequest request,
                CreateProjectHandler handler,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateProjectCommand(
                    request.Title,
                    request.ShortDescription,
                    request.Description,
                    request.ProjectUrl is null ? null : new Uri(request.ProjectUrl),
                    request.StartDate,
                    request.EndDate);

                var result = await handler.Handle(
                    command,
                    cancellationToken);

                return Results.Created(
                    $"/api/projects/{result.ProjectId}",
                    result);
            });

        return endpoints;
    }
}