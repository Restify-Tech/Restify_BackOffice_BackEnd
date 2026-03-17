using FluentValidation;
using Restify.BackOffice.Application.DTOs;

namespace Restify.BackOffice.Application.Validators;

public class CreateGlobalModifierRequestValidator : AbstractValidator<CreateGlobalModifierRequest>
{
    public CreateGlobalModifierRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("El tipo de modificador no es válido");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("El orden debe ser mayor o igual a 0");
    }
}

public class UpdateGlobalModifierRequestValidator : AbstractValidator<UpdateGlobalModifierRequest>
{
    public UpdateGlobalModifierRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("El tipo de modificador no es válido");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("El orden debe ser mayor o igual a 0");
    }
}

public class AssignGlobalModifierRequestValidator : AbstractValidator<AssignGlobalModifierRequest>
{
    public AssignGlobalModifierRequestValidator()
    {
        RuleFor(x => x.GlobalModifierId)
            .NotEmpty().WithMessage("El ID del modificador es requerido");
    }
}
