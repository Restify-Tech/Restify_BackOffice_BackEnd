using FluentValidation;
using Restify.Core.Application.DTOs.General;

namespace Restify.Core.Application.Validators;

public class CreateGeneralTableRequestValidator : AbstractValidator<CreateGeneralTableRequest>
{
    public CreateGeneralTableRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("El codigo es requerido")
            .MaximumLength(50).WithMessage("El codigo no puede exceder 50 caracteres")
            .Matches(@"^[a-zA-Z0-9_\-]+$").WithMessage("El codigo solo puede contener letras, numeros, guiones y guiones bajos");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("La descripcion no puede exceder 500 caracteres");

        RuleFor(x => x.ApplicationCode)
            .MaximumLength(50).WithMessage("El codigo de aplicacion no puede exceder 50 caracteres");

        RuleFor(x => x.Icon)
            .MaximumLength(100).WithMessage("El icono no puede exceder 100 caracteres");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("El orden de visualizacion debe ser mayor o igual a 0");
    }
}

public class UpdateGeneralTableRequestValidator : AbstractValidator<UpdateGeneralTableRequest>
{
    public UpdateGeneralTableRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID es requerido");

        Include(new CreateGeneralTableRequestValidator());
    }
}
