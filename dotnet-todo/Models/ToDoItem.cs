using System.ComponentModel.DataAnnotations.Schema;

namespace dotnet_todo.Models;

public class ToDoItem
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string? Content { get; set; }

    public bool IsComplete { get; set; } = false;
    public bool IsDeleted { get; set; } = false;

    public required List<Tag> Tags { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime LastUpdatedDate { get; set; } = DateTime.Now;

    public required PriorityType Priority { get; set; }
}