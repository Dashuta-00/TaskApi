using System.ComponentModel.DataAnnotations;
using TaskApi.Enums;

namespace TaskApi.Models;

public class TaskItem
{
    public Guid Id { get; set; }

    [Required] [MaxLength(200)] public string Title { get; set; } = null!;

    public bool IsCompleted { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? CompletedAt { get; set; }

    public Priority Priority { get; set; }
    
    public uint Version { get; set; }
}
