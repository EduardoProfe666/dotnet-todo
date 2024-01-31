using dotnet_todo.Dto.Tags;
using dotnet_todo.Models;
using FluentValidation;

namespace dotnet_todo.Validators.Tags;

public class TagCreatedDtoValidator : AbstractValidator<TagCreatedDto>
{
    public TagCreatedDtoValidator()
    {
        RuleFor(p => p.TagName)
            .MaximumLength(32)
            .WithMessage("El nombre debe tener una extensión máxima de 32 caracteres")
            .NotEmpty()
            .WithMessage("El nombre no debe estar vacío");
        
        RuleFor(p => p.TagDescription)
            .MaximumLength(128)
            .WithMessage("La descripción debe tener una extensión máxima de 128 caracteres")
            .NotEmpty()
            .WithMessage("La descripción no debe estar vacía");
    }
    
}