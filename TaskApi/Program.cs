using System.Reflection;
using System.Text.Json.Serialization;
using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using TaskApi.Data;
using TaskApi.Endpoints;
using TaskApi.Extensions;
using TaskApi.Services;
using TaskApi.Validators;

var builder = WebApplication.CreateBuilder(args);

var postgresConnection =
    builder.Configuration.GetConnectionString("Postgres")
    ?? throw new InvalidOperationException(
        "ConnectionStrings:Postgres is not configured.");


builder.Services.AddDbContext<TaskDbContext>(options => { options.UseNpgsql(postgresConnection); });

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter());
});

builder.Services.AddValidatorsFromAssemblyContaining<CreateTaskRequestValidator>();

TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

builder.Services.AddScoped<ITaskService, TaskService>();

builder.Services.AddMassTransitMessaging(
    builder.Configuration,
    builder.Environment);

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TaskDbContext>();

    await db.Database.EnsureCreatedAsync();
}

app.MapTaskEndpoint();

app.Run();


public partial class Program;