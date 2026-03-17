using FluentValidation;
using Restify.BackOffice.Application.DTOs;

namespace Restify.BackOffice.Application.Validators;

public class CreateTableRequestValidator : AbstractValidator<CreateTableRequest>
{
    public CreateTableRequestValidator()
    {
        RuleFor(x => x.Number)
            .NotEmpty().WithMessage("El número de mesa es requerido")
            .MaximumLength(10).WithMessage("El número no puede exceder 10 caracteres");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("La capacidad debe ser mayor a 0")
            .LessThanOrEqualTo(50).WithMessage("La capacidad no puede exceder 50");

        RuleFor(x => x.Name)
            .MaximumLength(50).WithMessage("El nombre no puede exceder 50 caracteres")
            .When(x => x.Name != null);

        RuleFor(x => x.Zone)
            .MaximumLength(50).WithMessage("La zona no puede exceder 50 caracteres")
            .When(x => x.Zone != null);
    }
}

public class UpdateTableRequestValidator : AbstractValidator<UpdateTableRequest>
{
    public UpdateTableRequestValidator()
    {
        RuleFor(x => x.Number)
            .NotEmpty().WithMessage("El número de mesa es requerido")
            .MaximumLength(10).WithMessage("El número no puede exceder 10 caracteres");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("La capacidad debe ser mayor a 0")
            .LessThanOrEqualTo(50).WithMessage("La capacidad no puede exceder 50");
    }
}

public class UpdateTableStatusRequestValidator : AbstractValidator<UpdateTableStatusRequest>
{
    public UpdateTableStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("El estado no es válido");
    }
}
