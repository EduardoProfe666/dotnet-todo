using System.Text.Json;
using System.Text.Json.Serialization;
using dotnet_todo.db;
using dotnet_todo.Dto.Filter;
using dotnet_todo.Dto.ToDoItem;
using dotnet_todo.Models;
using dotnet_todo.Validators;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace dotnet_todo.Endpoints;

public static class ToDoEndpoints
{
    public static void MapToDoEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/todo")
            .WithTags(["ToDo"])
            .WithName("ToDo")
            .WithOpenApi();

        group.MapGet("", GetToDos)
            .WithName("ToDo.Get")
            .WithSummary("Permite obtener todos los ToDos");
        
        group.MapGet("/{id:int}", GetToDoById)
            .WithName("ToDo.GetById")
            .WithSummary("Permite obtener un ToDo por su id");
        
        group.MapPost("", PostToDo)
            .WithName("ToDo.Post")
            .WithSummary("Permite crear un ToDo")
            .AddEndpointFilter<ValidationFilter<ToDoItemCreatedDto>>();
        
        group.MapPut("/{id:int}", PutToDo)
            .WithName("ToDo.Put")
            .WithSummary("Permite actualizar un ToDo")
            .AddEndpointFilter<ValidationFilter<ToDoItemUpdateDto>>();
        
        group.MapDelete("/{id:int}", DeleteToDo)
            .WithName("ToDo.Delete")
            .WithSummary("Permite eliminar un ToDo a la papelera");
        
        group.MapGet("/{isComplete:bool}", GetIsComplete)
            .WithName("ToDo.IsComplete")
            .WithSummary("Permite obtener todos los ToDos por su estado de completado");

        group.MapGet("/priority/{priority:int}", GetPriority)
            .WithName("ToDo.ByPriority")
            .WithSummary("Permite obtener todos los ToDos con una determinada prioridad");

        group.MapGet("/quantity/", GetQuantity)
            .WithName("ToDo.Quantity")
            .WithSummary("Permite obtener la cantidad de ToDos");

        group.MapGet("/{tagName}", GetToDosWithTag)
            .WithName("ToDo.ByTagName")
            .WithSummary("Permite obtener todos los ToDos con el tag dado");
        
        group.MapPost("/filter-sort/", FilterAndSort)
            .WithName("ToDo.FilterAndSort")
            .WithSummary("Permite filtrar y ordenar ToDos")
            .WithDescription("""
                              - SortOrder -> Asc(0) | Desc(1)
                              - SortBy -> title(0) | creation(1) | update(2)
                              """)
            .AddEndpointFilter<ValidationFilter<ToDoFilter>>();

        async Task<JsonHttpResult<List<ToDoItem>>> GetToDos(ToDoDb db, CancellationToken ct)
        {
            var data = await db.ToDos.Include(toDoItem => toDoItem.Tags).ToListAsync(ct);
            var options = new JsonSerializerOptions();
            options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            return TypedResults.Json(data, options);
        }
            

        async Task<Results<JsonHttpResult<ToDoItem>, NotFound<string>>> GetToDoById(int id, ToDoDb db, CancellationToken ct)
        {
            var todo = await db.ToDos.Include(toDoItem => toDoItem.Tags).FirstOrDefaultAsync((t) => t.Id == id, cancellationToken: ct);
            if (todo is null)
                return TypedResults.NotFound("No existe ningún ToDo con ese id");
            var options = new JsonSerializerOptions();
            options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            return TypedResults.Json(todo, options);
        }

        async Task<Results<NotFound<string>, BadRequest, Created>> PostToDo(ToDoItemCreatedDto todo, ToDoDb db,
            CancellationToken ct)
        {
            List<Tag> tags = new();
            if (todo.TagsId is not null)
            {
                foreach (var i in todo.TagsId!)
                {
                    var tag = await db.Tags.FirstOrDefaultAsync((t) => t.Id == i, cancellationToken: ct);
                    if (tag is null)
                        return TypedResults.NotFound("No existe ninguna etiqueta con ese id");
                    tags.Add(tag);
                }
            }
            
            await db.ToDos.AddAsync(new ToDoItem
                { Title = todo.Title, Content = todo.Content, Priority = todo.Priority, Tags = tags }, ct);
            await db.SaveChangesAsync(ct);
            return TypedResults.Created();
        }

        async Task<Results<NotFound<string>, BadRequest, Ok>> PutToDo(int id, ToDoItemUpdateDto item,
            ToDoDb db, CancellationToken ct)
        {
            var todo = await db.ToDos.FirstOrDefaultAsync((t) => t.Id == id, cancellationToken: ct);
            if (todo is null)
                return TypedResults.NotFound("No existe ningún ToDo con ese id");
            if (item.IsComplete is not null)
                todo.IsComplete = (bool)item.IsComplete;
            if (item.Priority is not null)
                todo.Priority = (PriorityType)item.Priority!;
            if (item.Content is not null)
                todo.Content = item.Content;
            if (item.Title is not null)
                todo.Title = item.Title;
            if (item.TagsId is not null)
            {
                List<Tag> tags = new();
                foreach (var i in item.TagsId!)
                {
                    var tag = await db.Tags.FirstOrDefaultAsync((t) => t.Id == i, cancellationToken: ct);
                    if (tag is null)
                        return TypedResults.NotFound("No existe ninguna etiqueta con ese id");
                    tags.Add(tag);
                }

                todo.Tags = tags;
            }
            
            todo.LastUpdatedDate = DateTime.Now;
            await db.SaveChangesAsync(ct);
            return TypedResults.Ok();
        }
        
        async Task<Results<NotFound<string>, Ok>> DeleteToDo(int id, ToDoDb db, CancellationToken ct)
        {
            var todo = await db.ToDos.FirstOrDefaultAsync((t) => t.Id == id, cancellationToken: ct);
            if (todo is null)
                return TypedResults.NotFound("No existe ningún ToDo con ese id");
            todo.LastUpdatedDate = DateTime.Now;
            todo.IsDeleted = true;
            await db.SaveChangesAsync(ct);
            return TypedResults.Ok();
        }

        async Task<JsonHttpResult<List<ToDoItem>>> GetIsComplete(bool isComplete, ToDoDb db, CancellationToken ct)
        {
            var data = await db.ToDos.Include(toDoItem => toDoItem.Tags).Where(t => t.IsComplete == isComplete).ToListAsync(ct);
            var options = new JsonSerializerOptions();
            options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            return TypedResults.Json(data, options);
        }


        async Task<JsonHttpResult<List<ToDoItem>>> GetPriority(PriorityType priority, ToDoDb db, CancellationToken ct)
        {
            var data = await db.ToDos.Include(toDoItem => toDoItem.Tags).Where(t => t.Priority == priority).ToListAsync(ct);
            var options = new JsonSerializerOptions();
            options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            return TypedResults.Json(data, options);
        }

        async Task<Ok<int>> GetQuantity(ToDoDb db, CancellationToken ct) =>
            TypedResults.Ok(await db.ToDos.CountAsync(ct));

        async Task < Results<JsonHttpResult<List<ToDoItem>>, NotFound<string>>> GetToDosWithTag(string tagName, ToDoDb db, CancellationToken ct)
        {
            var data = await db.Tags.Include(tag => tag.ToDoItems).FirstOrDefaultAsync((a) => a.TagName == tagName, cancellationToken: ct);
            if (data is null)
                return TypedResults.NotFound("No existe ninguna etiqueta con ese nombre");
            var options = new JsonSerializerOptions();
            options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            return TypedResults.Json(data.ToDoItems, options);
        }

        async Task<Results<BadRequest<string>, JsonHttpResult<IEnumerable<ToDoItem>>>> FilterAndSort(ToDoFilter filter, ToDoDb db,
            CancellationToken ct)
        {
            IEnumerable<ToDoItem> data = await db.ToDos.Include(toDoItem => toDoItem.Tags).ToListAsync(ct);
            if (filter.Title is not null)
                data = data.Where((t) => t.Title.Contains(filter.Title));
            if (filter.Content is not null)
                data = data.Where((t) => t.Content != null && t.Content.Contains(filter.Content));
            if (filter.CreatedDate!.HasValue)
                data = data.Where((t) => t.CreatedDate == filter.CreatedDate);
            if (filter.LastUpdatedDate!.HasValue)
                data = data.Where((t) => t.LastUpdatedDate == filter.LastUpdatedDate);
            if (filter.IsComplete is not null)
                data = data.Where((t) => t.IsComplete == filter.IsComplete);
            if (filter.Tags is not null)
                data = data.Where(t => t.Tags.Any((a) => filter.Tags!.Contains(a.Id)));
            
            switch (filter.SortBy)
            {
                case SortBy.Name:
                    data = filter.SortOrder == SortOrder.Asc
                        ? data.OrderBy(t => t.Title)
                        : data.OrderByDescending(t => t.Title);
                    break;
                case SortBy.CreatedDate:
                    data = filter.SortOrder == SortOrder.Asc
                        ? data.OrderBy(t => t.CreatedDate)
                        : data.OrderByDescending(t => t.CreatedDate);
                    break;
                case SortBy.UpdatedDate:
                    data = filter.SortOrder == SortOrder.Asc
                        ? data.OrderBy(t => t.LastUpdatedDate)
                        : data.OrderByDescending(t => t.LastUpdatedDate);
                    break;
            }
            var options = new JsonSerializerOptions();
            options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            return TypedResults.Json(data, options);
        }
        
    }
}