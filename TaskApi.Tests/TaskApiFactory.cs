using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskApi.Data;
using TaskApi.Messaging;

namespace TaskApi.Tests;

public class TaskApiFactory : WebApplicationFactory<Program>
{
    public FakeTaskCompletedPublisher Publisher { get; } = new();
    private readonly string _databaseName = $"task-api-tests-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<TaskDbContext>();

            services.RemoveAll<DbContextOptions<TaskDbContext>>();
            
            services.RemoveAll<IDbContextOptionsConfiguration<TaskDbContext>>();

            services.AddDbContext<TaskDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });
            
            services.RemoveAll<ITaskCompletedPublisher>();

            services.AddSingleton<ITaskCompletedPublisher>(Publisher);
        });
    }
}