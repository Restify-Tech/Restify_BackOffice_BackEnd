using FluentValidation;
using Restify.BackOffice.Application.DTOs;

namespace Restify.BackOffice.Application.Validators;

public class CreateEmployeeRequestValidator : AbstractValidator<CreateEmployeeRequest>
{
    public CreateEmployeeRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("El apellido es requerido")
            .MaximumLength(100).WithMessage("El apellido no puede exceder 100 caracteres");

        RuleFor(x => x.IdentificationNumber)
            .NotEmpty().WithMessage("El número de identificación es requerido")
            .MaximumLength(20).WithMessage("La identificación no puede exceder 20 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido")
            .EmailAddress().WithMessage("El formato del email no es válido");

        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("El teléfono no puede exceder 20 caracteres")
            .When(x => x.Phone != null);

        RuleFor(x => x.Position)
            .NotEmpty().WithMessage("El cargo es requerido")
            .MaximumLength(100).WithMessage("El cargo no puede exceder 100 caracteres");

        RuleFor(x => x.BaseSalary)
            .GreaterThan(0).WithMessage("El salario base debe ser mayor a 0");

        RuleFor(x => x.EmploymentType)
            .IsInEnum().WithMessage("El tipo de empleo no es válido");
    }
}

public class UpdateEmployeeRequestValidator : AbstractValidator<UpdateEmployeeRequest>
{
    public UpdateEmployeeRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("El apellido es requerido")
            .MaximumLength(100).WithMessage("El apellido no puede exceder 100 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido")
            .EmailAddress().WithMessage("El formato del email no es válido");

        RuleFor(x => x.Position)
            .NotEmpty().WithMessage("El cargo es requerido")
            .MaximumLength(100).WithMessage("El cargo no puede exceder 100 caracteres");

        RuleFor(x => x.BaseSalary)
            .GreaterThan(0).WithMessage("El salario base debe ser mayor a 0");

        RuleFor(x => x.EmploymentType)
            .IsInEnum().WithMessage("El tipo de empleo no es válido");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("El estado no es válido");
    }
}
