using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskApi.Contracts;
using TaskApi.Enums;

namespace TaskApi.Tests;

public class CompleteTaskIntegrationTest : IClassFixture<TaskApiFactory>
{
    private readonly TaskApiFactory _factory;
    private readonly HttpClient _client;
    
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web)
        {
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

    public CompleteTaskIntegrationTest(
        TaskApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _factory.Publisher.Clear();
    }
    
    [Fact]
    public async Task Create_And_Complete_Task()
    {
        var request = new
        {
            title = "Buy milk",
            priority = "High"
        };

        var createResponse = await _client.PostAsJsonAsync("/tasks", request);

        Assert.Equal(HttpStatusCode.Created,
            createResponse.StatusCode);

        var createdTask = await createResponse.Content
                .ReadFromJsonAsync<TaskResponse>(JsonOptions);

        Assert.NotNull(createdTask);

        Assert.Equal("Buy milk", createdTask.Title);

        Assert.False(createdTask.IsCompleted);

        Assert.Null(createdTask.CompletedAt);

        Assert.Equal(Priority.High, createdTask.Priority);

        var completeResponse = await _client.PutAsync($"/tasks/{createdTask.Id}/complete", content: null);

        Assert.Equal(HttpStatusCode.OK, completeResponse.StatusCode);

        var completedTask = await completeResponse.Content
                .ReadFromJsonAsync<TaskResponse>(JsonOptions);

        Assert.NotNull(completedTask);

        Assert.True(completedTask.IsCompleted);

        Assert.NotNull(completedTask.CompletedAt);

        var publishedEvent =
            Assert.Single(_factory.Publisher.Messages);

        Assert.Equal(
            createdTask.Id,
            publishedEvent.TaskId);

        Assert.Equal("Buy milk", publishedEvent.Title);

        Assert.Equal(Priority.High, publishedEvent.Priority);

        Assert.Equal(completedTask.CompletedAt, publishedEvent.CompletedAt);
    }
}