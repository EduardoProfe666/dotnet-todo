namespace dotnet_todo.Models;

public class Tag
{
    public int Id { get; set; }
    
    public required string TagName { get; set; }
    
    public required string? TagDescription { get; set; }

    public List<ToDoItem> ToDoItems { get; set; } = new List<ToDoItem>();
}