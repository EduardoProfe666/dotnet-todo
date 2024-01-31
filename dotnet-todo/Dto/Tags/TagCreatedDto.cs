namespace dotnet_todo.Dto.Tags;

public class TagCreatedDto
{
    public required string TagName { get; set; }
    
    public required string? TagDescription { get; set; }
}