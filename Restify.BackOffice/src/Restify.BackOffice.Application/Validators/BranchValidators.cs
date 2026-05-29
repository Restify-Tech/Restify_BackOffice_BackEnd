using FluentValidation;
using Restify.BackOffice.Application.DTOs;

namespace Restify.BackOffice.Application.Validators;

public class CreateBranchRequestValidator : AbstractValidator<CreateBranchRequest>
{
    public CreateBranchRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre de la sucursal es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("El formato del email no es valido")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("El telefono no puede exceder 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("La direccion no puede exceder 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Address));

        RuleFor(x => x.City)
            .MaximumLength(100).WithMessage("La ciudad no puede exceder 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.City));
    }
}

public class UpdateBranchRequestValidator : AbstractValidator<UpdateBranchRequest>
{
    public UpdateBranchRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre de la sucursal es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("El formato del email no es valido")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("El telefono no puede exceder 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("La direccion no puede exceder 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Address));

        RuleFor(x => x.City)
            .MaximumLength(100).WithMessage("La ciudad no puede exceder 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.City));
    }
}

public class CreateManagerAssignmentRequestValidator : AbstractValidator<CreateManagerAssignmentRequest>
{
    public CreateManagerAssignmentRequestValidator()
    {
        RuleFor(x => x.BranchId)
            .NotEmpty().WithMessage("La sucursal es requerida");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("El usuario es requerido");

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("El nombre del usuario es requerido")
            .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres");

        RuleFor(x => x.UserEmail)
            .NotEmpty().WithMessage("El email del usuario es requerido")
            .EmailAddress().WithMessage("El formato del email no es valido")
            .MaximumLength(200).WithMessage("El email no puede exceder 200 caracteres");

        RuleFor(x => x.MaxDiscountPercent)
            .InclusiveBetween(0, 100).WithMessage("El descuento maximo debe estar entre 0 y 100");
    }
}
