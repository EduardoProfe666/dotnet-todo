using dotnet_todo.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_todo.db;

public class ToDoDb : DbContext
{
    public ToDoDb(DbContextOptions<ToDoDb> options) : base(options)
    {
    }

    public DbSet<ToDoItem> ToDos => Set<ToDoItem>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<ToDoItemTag> ToDoItemTags => Set<ToDoItemTag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ToDoItem>()
            .HasMany(e => e.Tags)
            .WithMany(e => e.ToDoItems)
            .UsingEntity<ToDoItemTag>();
    }
}