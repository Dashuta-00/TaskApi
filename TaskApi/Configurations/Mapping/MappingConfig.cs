using Mapster;
using TaskApi.Contracts;
using TaskApi.Enums;
using TaskApi.Models;

namespace TaskApi.Configurations.Mapping;

public sealed class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<TaskItem, TaskResponse>();

        config.NewConfig<CreateTaskRequest, TaskItem>()
            .Map(
                destination => destination.Id,
                _ => Guid.NewGuid())
            .Map(
                destination => destination.Title,
                source => source.Title!.Trim())
            .Map(
                destination => destination.IsCompleted,
                _ => false)
            .Map(
                destination => destination.CreatedAt,
                _ => DateTimeOffset.UtcNow)
            .Map(
                destination => destination.CompletedAt,
                _ => (DateTimeOffset?)null)
            .Map(
                destination => destination.Priority,
                source => source.Priority ?? Priority.Medium)
            .Ignore(destination => destination.Version);
    }
}