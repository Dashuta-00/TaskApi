using Mapster;
using Microsoft.EntityFrameworkCore;
using TaskApi.Contracts;
using TaskApi.Contracts.Result;
using TaskApi.Data;
using TaskApi.Messaging;
using TaskApi.Models;

namespace TaskApi.Services;

public class TaskService(
    TaskDbContext db,
    ITaskCompletedPublisher publisher) : ITaskService
{
    public async Task<Result<TaskResponse>> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var task = request.Adapt<TaskItem>();

        db.Tasks.Add(task);
        await db.SaveChangesAsync(cancellationToken);

        return Result<TaskResponse>.Success(
            TaskResponse.From(task));
    }

    public async Task<Result<IReadOnlyCollection<TaskResponse>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var tasks = await db.Tasks
            .AsNoTracking()
            .OrderBy(x => x.CreatedAt)
            .ProjectToType<TaskResponse>()
            .ToListAsync(cancellationToken);
        
        return Result<IReadOnlyCollection<TaskResponse>>
            .Success(tasks);
    }

    public async Task<Result<TaskResponse>> CompleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var task = await db.Tasks
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (task is null)
        {
            return Result<TaskResponse>.NotFound("Task was not found.");
        }
        
        if (task.IsCompleted)
        {
            return Result<TaskResponse>
                .Conflict("Task is already completed.");
        }

        task.IsCompleted = true;

        task.CompletedAt = DateTimeOffset.UtcNow;

        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result<TaskResponse>.Conflict("Task was already modified by another request.");
        }

        await publisher.PublishAsync(
            new TaskCompletedEvent(
                task.Id,
                task.Title,
                task.CompletedAt.Value,
                task.Priority));

        return Result<TaskResponse>.Success(TaskResponse.From(task));
    }

    public async Task<Result<DeleteTaskResult>> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var task = await db.Tasks
            .SingleOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (task is null)
        {
            return Result<DeleteTaskResult>.NotFound("Task was not found.");
        }

        db.Tasks.Remove(task);

        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result<DeleteTaskResult>.Conflict(
                "Task was modified concurrently.");
        }
        
        return Result<DeleteTaskResult>.Success();
    }
}