using FluentValidation;
using Restify.Core.Application.DTOs.DataProvider;

namespace Restify.Core.Application.Validators;

public class DataProviderQueryRequestValidator : AbstractValidator<DataProviderQueryRequest>
{
    public DataProviderQueryRequestValidator()
    {
        RuleFor(x => x.EntityName)
            .NotEmpty().WithMessage("El nombre de la entidad es requerido")
            .MaximumLength(100).WithMessage("El nombre de la entidad no puede exceder 100 caracteres");

        RuleFor(x => x.ViewName)
            .NotEmpty().WithMessage("El nombre de la vista es requerido")
            .MaximumLength(100).WithMessage("El nombre de la vista no puede exceder 100 caracteres");

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("La pagina debe ser mayor o igual a 1");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("El tamano de pagina debe estar entre 1 y 100");

        RuleFor(x => x.SortField)
            .MaximumLength(100).WithMessage("El campo de ordenamiento no puede exceder 100 caracteres");

        RuleFor(x => x.SortDirection)
            .Must(x => x == "asc" || x == "desc" || string.IsNullOrEmpty(x))
            .WithMessage("La direccion de ordenamiento debe ser 'asc' o 'desc'");

        RuleFor(x => x.GlobalSearch)
            .MaximumLength(200).WithMessage("La busqueda global no puede exceder 200 caracteres");
    }
}

public class DataProviderMetadataRequestValidator : AbstractValidator<DataProviderMetadataRequest>
{
    public DataProviderMetadataRequestValidator()
    {
        RuleFor(x => x.EntityName)
            .NotEmpty().WithMessage("El nombre de la entidad es requerido")
            .MaximumLength(100).WithMessage("El nombre de la entidad no puede exceder 100 caracteres");

        RuleFor(x => x.ViewName)
            .NotEmpty().WithMessage("El nombre de la vista es requerido")
            .MaximumLength(100).WithMessage("El nombre de la vista no puede exceder 100 caracteres");
    }
}
