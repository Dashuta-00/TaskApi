using TaskApi.Contracts;
using TaskApi.Contracts.Result;

namespace TaskApi.Services;

public interface ITaskService
{
    Task<Result<TaskResponse>> CreateAsync(
        CreateTaskRequest request,
        CancellationToken cancellationToken);

    Task<Result<IReadOnlyCollection<TaskResponse>>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<Result<TaskResponse>> CompleteAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<Result<DeleteTaskResult>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);
}