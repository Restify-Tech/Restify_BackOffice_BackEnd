using FluentValidation;
using Restify.Core.Application.DTOs.Grid;

namespace Restify.Core.Application.Validators;

public class CreateGridConfigurationRequestValidator : AbstractValidator<CreateGridConfigurationRequest>
{
    public CreateGridConfigurationRequestValidator()
    {
        RuleFor(x => x.EntityName)
            .NotEmpty().WithMessage("El nombre de la entidad es requerido")
            .MaximumLength(100).WithMessage("El nombre de la entidad no puede exceder 100 caracteres");

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("El nombre de visualizacion es requerido")
            .MaximumLength(200).WithMessage("El nombre de visualizacion no puede exceder 200 caracteres");

        RuleFor(x => x.DisplayNamePlural)
            .NotEmpty().WithMessage("El nombre plural es requerido")
            .MaximumLength(200).WithMessage("El nombre plural no puede exceder 200 caracteres");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("La descripcion no puede exceder 500 caracteres");

        RuleFor(x => x.Icon)
            .MaximumLength(100).WithMessage("El icono no puede exceder 100 caracteres");

        RuleFor(x => x.ApiEndpoint)
            .NotEmpty().WithMessage("El endpoint de API es requerido")
            .MaximumLength(500).WithMessage("El endpoint no puede exceder 500 caracteres");

        RuleFor(x => x.DefaultPageSize)
            .InclusiveBetween(5, 100).WithMessage("El tamano de pagina debe estar entre 5 y 100");

        RuleFor(x => x.DefaultSortColumn)
            .MaximumLength(100).WithMessage("La columna de ordenamiento no puede exceder 100 caracteres");
    }
}

public class UpdateGridConfigurationRequestValidator : AbstractValidator<UpdateGridConfigurationRequest>
{
    public UpdateGridConfigurationRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID es requerido");

        Include(new CreateGridConfigurationRequestValidator());
    }
}
