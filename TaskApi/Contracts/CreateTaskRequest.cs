using TaskApi.Enums;

namespace TaskApi.Contracts;

public sealed record CreateTaskRequest(
    string? Title, 
    Priority? Priority);