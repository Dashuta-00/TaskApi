using TaskApi.Contracts;

namespace TaskApi.Messaging;

public interface ITaskCompletedPublisher
{
    Task PublishAsync(TaskCompletedEvent message);
}