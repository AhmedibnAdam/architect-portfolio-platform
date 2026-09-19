

using Application.Portfolio.Projects.UpdateProject;

namespace Api.Projects.updateProject;

public static class UpdateProjectEndpoint
{
    public static IEndpointRouteBuilder MapUpdateProjectEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut(
            "/api/projects/{projectId}",
            async (
                Guid projectId,
                UpdateProjectRequest request,
                UpdateProjectHandler handler,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateProjectCommand(
                    projectId,
                    request.Title,
                    request.ShortDescription,
                    request.Description,
                    request.ProjectUrl is null ? null : new Uri(request.ProjectUrl),
                    request.StartDate,
                    request.EndDate);

                var result = await handler.Handle(
                    command,
                    cancellationToken);

                return Results.Ok(result);
            });

        return endpoints;
    }
}
