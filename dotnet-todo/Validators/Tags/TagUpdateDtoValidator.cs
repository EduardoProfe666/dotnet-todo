using dotnet_todo.Dto.Tags;
using FluentValidation;

namespace dotnet_todo.Validators.Tags;

public class TagUpdateDtoValidator : AbstractValidator<TagUpdateDto>
{
    public TagUpdateDtoValidator()
    {
        RuleFor(p => p.TagDescription)
            .MaximumLength(128)
            .WithMessage("La descripción debe tener una extensión máxima de 128 caracteres")
            .NotEmpty()
            .WithMessage("La descripción no debe estar vacía");
    }
}