using System.Collections.Concurrent;
using TaskApi.Contracts;
using TaskApi.Messaging;

namespace TaskApi.Tests;

public class FakeTaskCompletedPublisher : ITaskCompletedPublisher
{
    private readonly ConcurrentQueue<TaskCompletedEvent> _messages = new();

    public IReadOnlyCollection<TaskCompletedEvent> Messages => [.. _messages];
    
    
    public Task PublishAsync(TaskCompletedEvent message)
    {
        _messages.Enqueue(message);

        return Task.CompletedTask;
    }
    
    public void Clear()
    {
        while (_messages.TryDequeue(out _))
        {
        }
    }
}