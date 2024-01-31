using FluentValidation;

namespace dotnet_todo.Validators;

public class ValidationFilter<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext ctx, EndpointFilterDelegate next)
    {
        var validator = ctx.HttpContext.RequestServices.GetService<IValidator<T>>();
        if (validator is null) return await next(ctx);
        
        var entity = ctx.Arguments
            .OfType<T>()
            .FirstOrDefault(a => a?.GetType() == typeof(T));
        if (entity is null) return TypedResults.Problem("No se pudo encontrar el tipo para validar");
        
        var validation = await validator.ValidateAsync(entity);
        if (validation.IsValid)
        {
            return await next(ctx);
        }

        return TypedResults.ValidationProblem(validation.ToDictionary());
    }
}