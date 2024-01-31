using dotnet_todo.db;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace dotnet_todo.Endpoints;

public static class GeneralEndpoints
{
    public static void MapGeneralEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("")
            .WithTags(["General"])
            .WithOpenApi();

        app.MapGet("/", () => Results.Redirect("/swagger/index.html")).ExcludeFromDescription();
        app.MapGet("/docs", () => Results.Redirect("/swagger/index.html")).ExcludeFromDescription();

        group.MapGet("/backup", Backup)
            .WithSummary("Permite hacer un backup de todos los ToDos, la papelera y las etiquetas");

        async Task<Ok<Dictionary<string, object>>> Backup(ToDoDb db, CancellationToken ct)
        {
            var todo = await db.ToDos.Where(e => !e.IsDeleted).ToListAsync(cancellationToken: ct);
            var bin = await db.ToDos.Where(e => e.IsDeleted).ToListAsync(cancellationToken: ct);
            var tags = await db.Tags.ToListAsync(cancellationToken: ct);
            return TypedResults.Ok(new Dictionary<string, object>()
            {
                { "ToDos", todo },
                { "Papelera", bin },
                { "Etiquetas", tags }
            });
        }
    }
}