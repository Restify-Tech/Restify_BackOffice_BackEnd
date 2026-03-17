using FluentValidation;
using Restify.BackOffice.Application.DTOs;

namespace Restify.BackOffice.Application.Validators;

public class CreateSupplierRequestValidator : AbstractValidator<CreateSupplierRequest>
{
    public CreateSupplierRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres");

        RuleFor(x => x.TaxId)
            .MaximumLength(20).WithMessage("El RUC/NIT no puede exceder 20 caracteres")
            .When(x => x.TaxId != null);

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("El formato del email no es válido")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("El teléfono no puede exceder 20 caracteres")
            .When(x => x.Phone != null);
    }
}

public class UpdateSupplierRequestValidator : AbstractValidator<UpdateSupplierRequest>
{
    public UpdateSupplierRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres");

        RuleFor(x => x.TaxId)
            .MaximumLength(20).WithMessage("El RUC/NIT no puede exceder 20 caracteres")
            .When(x => x.TaxId != null);

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("El formato del email no es válido")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("El teléfono no puede exceder 20 caracteres")
            .When(x => x.Phone != null);
    }
}
