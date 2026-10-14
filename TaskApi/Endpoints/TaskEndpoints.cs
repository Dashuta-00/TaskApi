using TaskApi.Contracts;
using TaskApi.Extensions;
using TaskApi.Filters;
using TaskApi.Services;

namespace TaskApi.Endpoints;

public static class TaskEndpoints
{
    public static IEndpointRouteBuilder MapTaskEndpoint(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("tasks");

        group.MapPost(string.Empty, CreateTaskAsync).AddEndpointFilter<ValidationFilter<CreateTaskRequest>>();
        group.MapGet(string.Empty, GetAllTasksAsync);
        group.MapPut("/{id:guid}/complete", CompleteTaskAsync);
        group.MapDelete("/{id:guid}", DeleteTaskAsync);
        
        return builder;
    }

    private static async Task<IResult> CreateTaskAsync(
        CreateTaskRequest request,
        ITaskService taskService,
        CancellationToken cancellationToken)
    {
        var result = await taskService.CreateAsync(
            request,
            cancellationToken);

        return result.ToResult(task => Results.Created(
            $"/tasks/{task.Id}",
            task));
    }


    private static async Task<IResult> GetAllTasksAsync(
        ITaskService taskService,
        CancellationToken cancellationToken)
    {
        var result =
            await taskService.GetAllAsync(
                cancellationToken);

        return result.ToResult(Results.Ok);
    }

    private static async Task<IResult> CompleteTaskAsync(
        Guid id,
        ITaskService taskService,
        CancellationToken cancellationToken)
    {
        var result =
            await taskService.CompleteAsync(
                id,
                cancellationToken);

        return result.ToResult(Results.Ok);
    }

    private static async Task<IResult> DeleteTaskAsync(
        Guid id,
        ITaskService taskService,
        CancellationToken cancellationToken)
    {
        var result =
            await taskService.DeleteAsync(
                id,
                cancellationToken);

        return result.ToResult(_ => Results.NoContent());
    }
}