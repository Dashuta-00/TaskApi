using System.Globalization;
using Microsoft.EntityFrameworkCore;
using TaskApi.Models;

namespace TaskApi.Data;

public class TaskDbContext : DbContext
{
    public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options){}
    
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var task = modelBuilder.Entity<TaskItem>();
        
        task.ToTable("tasks");
        task.HasKey(x => x.Id);

        task.Property(x => x.Id)
            .HasColumnName("id");
        
        task.Property(x => x.Title)
            .HasColumnName("title")
            .IsRequired()
            .HasMaxLength(200);

        task.Property(x => x.IsCompleted)
            .HasColumnName("is_completed")
            .IsRequired();

        task.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        task.Property(x => x.CompletedAt)
            .HasColumnName("completed_at");

        task.Property(x => x.Priority)
            .HasColumnName("priority")
            .IsRequired();

        task.Property(x => x.Version)
            .IsRowVersion();
    }
}