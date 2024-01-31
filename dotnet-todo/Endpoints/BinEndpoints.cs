using dotnet_todo.db;
using dotnet_todo.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace dotnet_todo.Endpoints;

public static class BinEndpoints
{
    public static void MapBinEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/bin")
            .WithTags(["Papelera"])
            .WithName("Bin")
            .WithOpenApi();

        group.MapGet("", GetBin)
            .WithName("Bin.Get")
            .WithSummary("Permite obtener todos los ToDos de la papelera");

        group.MapGet("/{id:int}", GetById)
            .WithName("Bin.GetById")
            .WithSummary("Permite obtener un ToDo específico de la papelera");

        group.MapDelete("/restore/{id:int}", Restore)
            .WithName("Bin.Restore")
            .WithSummary("Permite recuperar un ToDo eliminado de la papelera");
        
        group.MapDelete("/{id:int}", Delete)
            .WithName("Bin.Delete")
            .WithSummary("Permite eliminar un ToDo de la papelera");
        
        group.MapGet("/quantity/", Quantity)
            .WithName("Bin.GetQuantity")
            .WithSummary("Permite obtener la cantidad de ToDos de la papelera");
        
        async Task<Ok<List<ToDoItem>>> GetBin(ToDoDb db, CancellationToken ct) => 
            TypedResults.Ok(await db.ToDos.Where(a => a.IsDeleted).ToListAsync(ct));

        async Task<Results<Ok<ToDoItem>, NotFound<string>>> GetById(int id, ToDoDb db, CancellationToken ct)
        {
            var todo = await db.ToDos.FirstOrDefaultAsync((t) => t.Id == id && t.IsDeleted, cancellationToken: ct);
            if (todo is null)
                return TypedResults.NotFound("No existe ningún ToDo con ese id en la papelera");
            return TypedResults.Ok(todo);
        }

        async Task<Results<NotFound<string>, Ok>> Restore(int id, ToDoDb db, CancellationToken ct)
        {
            var todo = await db.ToDos.FirstOrDefaultAsync((t) => t.Id == id && t.IsDeleted, cancellationToken: ct);
            if (todo is null)
                return TypedResults.NotFound("No existe ningún ToDo con ese id en la papelera");
            todo.LastUpdatedDate = DateTime.Now;
            todo.IsDeleted = false;
            await db.SaveChangesAsync(ct);
            return TypedResults.Ok();
        }

        async Task<Results<NotFound<string>, Ok>> Delete(int id, ToDoDb db, CancellationToken ct)
        {
            var todo = await db.ToDos.FirstOrDefaultAsync((t) => t.Id == id && t.IsDeleted, cancellationToken: ct);   
            if (todo is null)
                return TypedResults.NotFound("No existe ningún ToDo con ese id en la papelera");
            db.ToDos.Remove(todo);
            await db.SaveChangesAsync(ct);
            return TypedResults.Ok();
        }

        async Task<Ok<int>> Quantity(ToDoDb db, CancellationToken ct) =>
           TypedResults.Ok(await db.ToDos.Where(a=>a.IsDeleted).CountAsync(cancellationToken: ct));
    }
}