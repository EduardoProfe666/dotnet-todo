namespace dotnet_todo.Models;

public class ToDoItemTag
{
    public required int ToDoItemId { get; set; }
    public required int TagId { get; set; }
    public required ToDoItem ToDoItem { get; set; }
    public required Tag Tag { get; set; }
}