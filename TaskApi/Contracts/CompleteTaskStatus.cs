namespace TaskApi.Contracts;

public enum CompleteTaskStatus
{
    Success,
    NotFound,
    AlreadyCompleted,
    Conflict
}