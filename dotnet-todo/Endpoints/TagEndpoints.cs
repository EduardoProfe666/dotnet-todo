using System.Text.Json;
using System.Text.Json.Serialization;
using dotnet_todo.db;
using dotnet_todo.Dto.Tags;
using dotnet_todo.Models;
using dotnet_todo.Validators;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace dotnet_todo.Endpoints;

public static class TagEndpoints
{
    public static void MapTagEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/tag")
            .WithTags(["Etiquetas"])
            .WithName("Tags")
            .WithOpenApi();

        group.MapGet("", GetTags)
            .WithName("Tags.Get")
            .WithSummary("Permite obtener la información de todas las etiquetas");

        group.MapGet("/{id:int}", GetTagById)
            .WithName("Tags.GetById")
            .WithSummary("Permite obtener la información de una etiqueta determinada, con los ToDos relacionados");

        group.MapPost("", PostTag)
            .WithName("Tags.Create")
            .WithSummary("Crea una etiqueta")
            .AddEndpointFilter<ValidationFilter<TagCreatedDto>>();

        group.MapPatch("/{id:int}", PatchTag)
            .WithName("Tags.Modify")
            .WithSummary("Modifica la descripción de una etiqueta")
            .AddEndpointFilter<ValidationFilter<TagUpdateDto>>();

        group.MapDelete("/{id:int}", DeleteTag)
            .WithName("Tags.Delete")
            .WithSummary("Borra permanentemente una etiqueta");

        group.MapGet("/quantity/", Quantity)
            .WithName("Tags.Quantity")
            .WithSummary("Permite obtener la cantidad de etiquetas");

        async Task<Ok<List<Tag>>> GetTags(ToDoDb db, CancellationToken ct)
        {
            var data = await db.Tags.ToListAsync(ct);
            
            return TypedResults.Ok(data);
        }

        async Task<Results<JsonHttpResult<Tag>, NotFound<string>>> GetTagById(int id, ToDoDb db, CancellationToken ct)
        {
            var tag = await db.Tags.Include(tag => tag.ToDoItems).FirstOrDefaultAsync((t) => t.Id == id, ct);
            var options = new JsonSerializerOptions();
            options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            return tag is null
                ? TypedResults.NotFound("No existe ninguna etiqueta con ese id")
                : TypedResults.Json(tag, options);
        }
        

        async Task<Results<Created, BadRequest, Conflict<string>>> PostTag(TagCreatedDto tag, ToDoDb db, CancellationToken ct)
        {
            if (db.Tags.ToList().Exists(t => t.TagName.Equals(tag.TagName)))
                return TypedResults.Conflict("El nombre de la etiqueta debe ser único");
            var newData = new Tag { TagDescription = tag.TagDescription, TagName = tag.TagName };
            await db.Tags.AddAsync(newData, ct);
            await db.SaveChangesAsync(ct);
            return TypedResults.Created();
        }

        async Task<Results<NotFound<string>, Ok>> PatchTag(int id, TagUpdateDto tg, ToDoDb db, CancellationToken ct)
        {
            var tag = await db.Tags.FirstOrDefaultAsync((t) => t.Id == id, cancellationToken: ct);
            if (tag is null)
                return TypedResults.NotFound("No existe ninguna etiqueta con ese id");

            tag.TagDescription = tg.TagDescription;
            await db.SaveChangesAsync(ct);
            return TypedResults.Ok();
        }

        async Task<Results<NotFound<string>, Ok, Conflict<string>>> DeleteTag(int id, ToDoDb db, CancellationToken ct)
        {
            var tag = await db.Tags.Include(tag => tag.ToDoItems).FirstOrDefaultAsync((t) => t.Id == id, cancellationToken: ct);
            if (tag is null)
                return TypedResults.NotFound("No existe ninguna etiqueta con ese id");

            if (tag.ToDoItems.Count != 0)
                return TypedResults.Conflict($"La etiqueta está siendo referenciada por {tag.ToDoItems.Count} ToDos");
            
            db.Tags.Remove(tag);
            await db.SaveChangesAsync(ct);
            return TypedResults.Ok();
        }
        
        async Task<Ok<int>> Quantity(ToDoDb db, CancellationToken ct) =>
            TypedResults.Ok(await db.Tags.CountAsync(cancellationToken: ct));
    }
}