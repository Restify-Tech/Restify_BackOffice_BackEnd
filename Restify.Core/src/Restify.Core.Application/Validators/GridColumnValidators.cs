using FluentValidation;
using Restify.Core.Application.DTOs.Grid;

namespace Restify.Core.Application.Validators;

public class CreateGridColumnRequestValidator : AbstractValidator<CreateGridColumnRequest>
{
    public CreateGridColumnRequestValidator()
    {
        RuleFor(x => x.GridConfigurationId)
            .NotEmpty().WithMessage("El ID de configuracion de grid es requerido");

        RuleFor(x => x.FieldName)
            .NotEmpty().WithMessage("El nombre del campo es requerido")
            .MaximumLength(100).WithMessage("El nombre del campo no puede exceder 100 caracteres");

        RuleFor(x => x.HeaderText)
            .NotEmpty().WithMessage("El texto del encabezado es requerido")
            .MaximumLength(200).WithMessage("El texto del encabezado no puede exceder 200 caracteres");

        RuleFor(x => x.FormLabel)
            .MaximumLength(200).WithMessage("La etiqueta del formulario no puede exceder 200 caracteres");

        RuleFor(x => x.GridOrder)
            .GreaterThanOrEqualTo(0).WithMessage("El orden en el grid debe ser mayor o igual a 0");

        RuleFor(x => x.FormOrder)
            .GreaterThanOrEqualTo(0).WithMessage("El orden en el formulario debe ser mayor o igual a 0");
    }
}

public class CreateGridColumnValidationRequestValidator : AbstractValidator<CreateGridColumnValidationRequest>
{
    public CreateGridColumnValidationRequestValidator()
    {
        RuleFor(x => x.GridColumnId)
            .NotEmpty().WithMessage("El ID de columna es requerido");

        RuleFor(x => x.ValidationType)
            .NotEmpty().WithMessage("El tipo de validacion es requerido")
            .MaximumLength(50).WithMessage("El tipo de validacion no puede exceder 50 caracteres");

        RuleFor(x => x.ValidationValue)
            .MaximumLength(500).WithMessage("El valor de validacion no puede exceder 500 caracteres");

        RuleFor(x => x.ValidationValue2)
            .MaximumLength(500).WithMessage("El segundo valor de validacion no puede exceder 500 caracteres");

        RuleFor(x => x.ErrorMessage)
            .MaximumLength(500).WithMessage("El mensaje de error no puede exceder 500 caracteres");

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0).WithMessage("El orden debe ser mayor o igual a 0");
    }
}

public class CreateGridColumnLookupRequestValidator : AbstractValidator<CreateGridColumnLookupRequest>
{
    public CreateGridColumnLookupRequestValidator()
    {
        RuleFor(x => x.GridColumnId)
            .NotEmpty().WithMessage("El ID de columna es requerido");

        RuleFor(x => x.TargetEntity)
            .NotEmpty().WithMessage("La entidad destino es requerida")
            .MaximumLength(100).WithMessage("La entidad destino no puede exceder 100 caracteres");

        RuleFor(x => x.ApiEndpoint)
            .NotEmpty().WithMessage("El endpoint de API es requerido")
            .MaximumLength(500).WithMessage("El endpoint no puede exceder 500 caracteres");

        RuleFor(x => x.ValueField)
            .NotEmpty().WithMessage("El campo de valor es requerido")
            .MaximumLength(100).WithMessage("El campo de valor no puede exceder 100 caracteres");

        RuleFor(x => x.DisplayField)
            .NotEmpty().WithMessage("El campo de visualizacion es requerido")
            .MaximumLength(100).WithMessage("El campo de visualizacion no puede exceder 100 caracteres");
    }
}
