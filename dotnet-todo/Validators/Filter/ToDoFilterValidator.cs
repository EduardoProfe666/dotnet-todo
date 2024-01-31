using dotnet_todo.Dto.Filter;
using FluentValidation;

namespace dotnet_todo.Validators.Filter;

public class ToDoFilterValidator : AbstractValidator<ToDoFilter>
{
    public ToDoFilterValidator()
    {
        RuleFor(a => a.Title)
            .MaximumLength(100)
            .WithMessage("El título debe tener una extensión máxima de 100 caracteres")
            .NotEmpty()
            .WithMessage("El título no debe estar vacío");

        RuleFor(a => a.Content)
            .MaximumLength(1000)
            .WithMessage("La descripción debe tener una extensión máxima de 1000 caracteres")
            .NotEmpty()
            .WithMessage("La descripción no debe estar vacía");

        RuleFor(a => a.Priority)
            .IsInEnum()
            .WithMessage("La prioridad debe ser un valor válido");
        RuleFor(a => a.SortOrder)
            .IsInEnum()
            .WithMessage("El SortOrder debe ser un valor válido");
        RuleFor(a => a.SortBy)
            .IsInEnum()
            .WithMessage("El SortBy debe ser un valor válido");
    }
}