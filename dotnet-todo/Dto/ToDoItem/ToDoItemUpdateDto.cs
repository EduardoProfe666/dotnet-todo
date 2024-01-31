using dotnet_todo.Models;

namespace dotnet_todo.Dto.ToDoItem;

public class ToDoItemUpdateDto
{
    public required string? Title { get; set; }
    public required bool? IsComplete { get; set; } = false;
    public required string? Content { get; set; }
    public required List<int>? TagsId { get; set; }
    public required PriorityType? Priority { get; set; }
}