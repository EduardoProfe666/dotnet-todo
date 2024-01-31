using dotnet_todo.Models;

namespace dotnet_todo.Dto.Filter;

public class ToDoFilter
{
    public required string? Title { get; set; }
    
    public required string? Content { get; set; }

    public required bool? IsComplete { get; set; }
    public required List<int>? Tags { get; set; }
    public required DateTime? CreatedDate { get; set; }
    public required DateTime? LastUpdatedDate { get; set; }

    public required PriorityType? Priority { get; set; }
    
    public required SortBy? SortBy { get; set; }
    public required SortOrder? SortOrder { get; set; }
}