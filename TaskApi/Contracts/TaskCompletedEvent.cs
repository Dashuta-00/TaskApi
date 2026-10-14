using TaskApi.Enums;

namespace TaskApi.Contracts;

public record TaskCompletedEvent(
    Guid TaskId,
    string Title,
    DateTimeOffset CompletedAt,
    Priority Priority);