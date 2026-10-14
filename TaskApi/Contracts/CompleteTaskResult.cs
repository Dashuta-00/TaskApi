using TaskApi.Enums;

namespace TaskApi.Contracts;

public record CompleteTaskResult(
    CompleteTaskStatus Status,
    TaskResponse? Task = null);