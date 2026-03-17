using FluentValidation;
using Restify.BackOffice.Application.DTOs;

namespace Restify.BackOffice.Application.Validators;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("El precio debe ser mayor o igual a 0");

        RuleFor(x => x.Sku)
            .MaximumLength(50).WithMessage("El SKU no puede exceder 50 caracteres")
            .When(x => x.Sku != null);

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("La categoría es requerida");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("El orden debe ser mayor o igual a 0");

        RuleForEach(x => x.Modifiers)
            .SetValidator(new CreateProductModifierRequestValidator())
            .When(x => x.Modifiers != null);
    }
}

public class CreateProductModifierRequestValidator : AbstractValidator<CreateProductModifierRequest>
{
    public CreateProductModifierRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del modificador es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");
    }
}

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("El precio debe ser mayor o igual a 0");

        RuleFor(x => x.Sku)
            .MaximumLength(50).WithMessage("El SKU no puede exceder 50 caracteres")
            .When(x => x.Sku != null);

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("La categoría es requerida");

        RuleForEach(x => x.Modifiers)
            .SetValidator(new CreateProductModifierRequestValidator())
            .When(x => x.Modifiers != null);
    }
}
