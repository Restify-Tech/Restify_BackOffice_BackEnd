using FluentValidation;
using Restify.BackOffice.Application.DTOs;

namespace Restify.BackOffice.Application.Validators;

public class CreateAccountingAccountRequestValidator : AbstractValidator<CreateAccountingAccountRequest>
{
    public CreateAccountingAccountRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("El código es requerido")
            .MaximumLength(20).WithMessage("El código no puede exceder 20 caracteres")
            .Matches(@"^[\d.]+$").WithMessage("El código solo puede contener números y puntos");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres");

        RuleFor(x => x.AccountType)
            .IsInEnum().WithMessage("El tipo de cuenta no es válido");
    }
}

public class UpdateAccountingAccountRequestValidator : AbstractValidator<UpdateAccountingAccountRequest>
{
    public UpdateAccountingAccountRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres");
    }
}

public class CreateAccountingPeriodRequestValidator : AbstractValidator<CreateAccountingPeriodRequest>
{
    public CreateAccountingPeriodRequestValidator()
    {
        RuleFor(x => x.Year)
            .InclusiveBetween(2020, 2099).WithMessage("El año debe estar entre 2020 y 2099");

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12).WithMessage("El mes debe estar entre 1 y 12");
    }
}

public class CreateJournalEntryRequestValidator : AbstractValidator<CreateJournalEntryRequest>
{
    public CreateJournalEntryRequestValidator()
    {
        RuleFor(x => x.PeriodId)
            .NotEmpty().WithMessage("El periodo es requerido");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descripción es requerida")
            .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres");

        RuleFor(x => x.EntryType)
            .IsInEnum().WithMessage("El tipo de asiento no es válido");

        RuleFor(x => x.Lines)
            .NotEmpty().WithMessage("El asiento debe tener al menos una línea")
            .Must(lines => lines.Count >= 2).WithMessage("El asiento debe tener al menos 2 líneas");

        RuleFor(x => x.Lines)
            .Must(lines =>
            {
                var totalDebit = lines.Sum(l => l.Debit);
                var totalCredit = lines.Sum(l => l.Credit);
                return Math.Abs(totalDebit - totalCredit) < 0.01m;
            }).WithMessage("El asiento no está balanceado: débitos y créditos deben ser iguales");

        RuleForEach(x => x.Lines)
            .SetValidator(new CreateJournalEntryLineRequestValidator());
    }
}

public class CreateJournalEntryLineRequestValidator : AbstractValidator<CreateJournalEntryLineRequest>
{
    public CreateJournalEntryLineRequestValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("La cuenta es requerida");

        RuleFor(x => x)
            .Must(x => x.Debit > 0 || x.Credit > 0)
            .WithMessage("Cada línea debe tener un débito o crédito mayor a 0");

        RuleFor(x => x)
            .Must(x => !(x.Debit > 0 && x.Credit > 0))
            .WithMessage("Una línea no puede tener débito y crédito simultáneamente");
    }
}
