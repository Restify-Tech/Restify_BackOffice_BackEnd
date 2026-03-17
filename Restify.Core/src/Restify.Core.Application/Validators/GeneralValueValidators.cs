using FluentValidation;
using Restify.Core.Application.DTOs.General;

namespace Restify.Core.Application.Validators;

public class CreateGeneralValueRequestValidator : AbstractValidator<CreateGeneralValueRequest>
{
    public CreateGeneralValueRequestValidator()
    {
        RuleFor(x => x.GeneralTableId)
            .NotEmpty().WithMessage("El ID de tabla general es requerido");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("El codigo es requerido")
            .MaximumLength(50).WithMessage("El codigo no puede exceder 50 caracteres")
            .Matches(@"^[a-zA-Z0-9_\-]+$").WithMessage("El codigo solo puede contener letras, numeros, guiones y guiones bajos");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("El contenido es requerido")
            .MaximumLength(500).WithMessage("El contenido no puede exceder 500 caracteres");

        RuleFor(x => x.ShortDescription)
            .MaximumLength(200).WithMessage("La descripcion corta no puede exceder 200 caracteres");

        RuleFor(x => x.Reference1)
            .MaximumLength(500).WithMessage("La referencia 1 no puede exceder 500 caracteres");

        RuleFor(x => x.Reference2)
            .MaximumLength(500).WithMessage("La referencia 2 no puede exceder 500 caracteres");

        RuleFor(x => x.Reference3)
            .MaximumLength(500).WithMessage("La referencia 3 no puede exceder 500 caracteres");

        RuleFor(x => x.Reference4)
            .MaximumLength(500).WithMessage("La referencia 4 no puede exceder 500 caracteres");

        RuleFor(x => x.Reference5)
            .MaximumLength(500).WithMessage("La referencia 5 no puede exceder 500 caracteres");

        RuleFor(x => x.Icon)
            .MaximumLength(100).WithMessage("El icono no puede exceder 100 caracteres");

        RuleFor(x => x.BackgroundColor)
            .MaximumLength(20).WithMessage("El color de fondo no puede exceder 20 caracteres");

        RuleFor(x => x.TextColor)
            .MaximumLength(20).WithMessage("El color de texto no puede exceder 20 caracteres");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("El orden de visualizacion debe ser mayor o igual a 0");
    }
}

public class UpdateGeneralValueRequestValidator : AbstractValidator<UpdateGeneralValueRequest>
{
    public UpdateGeneralValueRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID es requerido");

        Include(new CreateGeneralValueRequestValidator());
    }
}

public class GetValuesByTableCodeRequestValidator : AbstractValidator<GetValuesByTableCodeRequest>
{
    public GetValuesByTableCodeRequestValidator()
    {
        RuleFor(x => x.TableCode)
            .NotEmpty().WithMessage("El codigo de tabla es requerido")
            .MaximumLength(50).WithMessage("El codigo de tabla no puede exceder 50 caracteres");
    }
}
