using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using MassTransit;
using TaskApi.Contracts;
using TaskApi.Messaging;

namespace TaskApi.Extensions;

public static class MassTransitExtensions
{
    public static IServiceCollection AddMassTransitMessaging(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddMassTransit(x =>
        {
            if (environment.IsEnvironment("Testing"))
            {
                x.UsingInMemory((_, _) =>
                {
                });

                return;
            }

            x.UsingRabbitMq((_, cfg) =>
            {
                var rabbitSection = configuration.GetSection("RabbitMQ");

                var hostName = rabbitSection["HostName"] ?? "localhost";
                var virtualHost = rabbitSection["VirtualHost"] ?? "/";
                var userName = rabbitSection["UserName"] ?? "guest";
                var password = rabbitSection["Password"] ?? "guest";

                cfg.Host(hostName, virtualHost, host =>
                {
                    host.Username(userName);
                    host.Password(password);

                    host.PublisherConfirmation = false;
                });

                cfg.Message<TaskCompletedEvent>(message =>
                {
                    message.SetEntityName("task.events");
                });

                cfg.Publish<TaskCompletedEvent>(publish =>
                {
                    publish.ExchangeType = "topic";
                    publish.Durable = true;
                    publish.AutoDelete = false;
                });

                cfg.AddRawJsonSerializer();

                cfg.Send<TaskCompletedEvent>(send =>
                {
                    send.UseRoutingKeyFormatter(_ => "task.completed");
                    send.UseSerializer(MediaTypeNames.Application.Json);
                });

                cfg.ConfigureJsonSerializerOptions(options =>
                {
                    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.Converters.Add(new JsonStringEnumConverter());
                    return options;
                });
            });
        });

        services.AddOptions<MassTransitHostOptions>()
            .Configure(options =>
            {
                options.WaitUntilStarted = false;
                options.StopTimeout = TimeSpan.FromSeconds(5);
            });

        services.Configure<HostOptions>(options =>
        {
            options.ShutdownTimeout = TimeSpan.FromSeconds(10);
        });

        services.AddScoped<
            ITaskCompletedPublisher,
            TaskCompletedPublisher>();

        return services;
    }
}