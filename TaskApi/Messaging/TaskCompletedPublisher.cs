using MassTransit;
using TaskApi.Contracts;

namespace TaskApi.Messaging;

public class TaskCompletedPublisher(
    IPublishEndpoint publishEndpoint,
    ILogger<TaskCompletedPublisher> logger) : ITaskCompletedPublisher
{
    public async Task PublishAsync(TaskCompletedEvent message)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

        try
        {
            await publishEndpoint.Publish(message, cts.Token); 
        }
        catch (OperationCanceledException ex)
        {
            logger.LogError(
                ex,
                "Publishing TaskCompleted event for task {TaskId} timed out",
                message.TaskId);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to publish TaskCompleted event for task {TaskId}",
                message.TaskId);
        }
    }
}