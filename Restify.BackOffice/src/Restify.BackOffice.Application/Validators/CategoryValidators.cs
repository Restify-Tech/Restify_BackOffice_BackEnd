using FluentValidation;
using Restify.BackOffice.Application.DTOs;

namespace Restify.BackOffice.Application.Validators;

public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres")
            .When(x => x.Description != null);

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("El orden debe ser mayor o igual a 0");
    }
}

public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID es requerido");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres")
            .When(x => x.Description != null);

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("El orden debe ser mayor o igual a 0");
    }
}
