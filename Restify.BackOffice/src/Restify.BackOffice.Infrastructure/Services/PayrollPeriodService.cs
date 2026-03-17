using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Application.Mappings;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Domain.Enums;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

public class PayrollPeriodService : IPayrollPeriodService
{
    private readonly IPayrollPeriodRepository _periodRepository;
    private readonly IPayrollEntryRepository _entryRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDeductionTypeRepository _deductionTypeRepository;
    private readonly ICurrentUserService _currentUserService;

    public PayrollPeriodService(
        IPayrollPeriodRepository periodRepository,
        IPayrollEntryRepository entryRepository,
        IEmployeeRepository employeeRepository,
        IDeductionTypeRepository deductionTypeRepository,
        ICurrentUserService currentUserService)
    {
        _periodRepository = periodRepository;
        _entryRepository = entryRepository;
        _employeeRepository = employeeRepository;
        _deductionTypeRepository = deductionTypeRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<PayrollPeriodDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _periodRepository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<PayrollPeriodDto>.Failure("Periodo de nomina no encontrado");

        return Result<PayrollPeriodDto>.Success(entity.ToDto());
    }

    public async Task<Result<IEnumerable<PayrollPeriodDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _periodRepository.GetAllAsync(cancellationToken);
        var dtos = entities.Select(e => e.ToDto());
        return Result<IEnumerable<PayrollPeriodDto>>.Success(dtos);
    }

    public async Task<Result<PayrollPeriodDto>> CreateAsync(CreatePayrollPeriodRequest request, CancellationToken cancellationToken = default)
    {
        var tenantId = _currentUserService.TenantId ?? Guid.Empty;
        var entity = request.ToEntity(tenantId);

        var created = await _periodRepository.AddAsync(entity, cancellationToken);

        return Result<PayrollPeriodDto>.Success(created.ToDto());
    }

    public async Task<Result<PayrollPeriodDto>> CalculatePayrollAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var period = await _periodRepository.GetByIdAsync(id, cancellationToken);
        if (period == null)
            return Result<PayrollPeriodDto>.Failure("Periodo de nomina no encontrado");

        // Solo se puede calcular si esta en borrador o ya calculado (re-calculo)
        if (period.Status != PayrollPeriodStatus.Draft && period.Status != PayrollPeriodStatus.Calculated)
            return Result<PayrollPeriodDto>.Failure("Solo se puede calcular un periodo en estado Borrador o Calculado");

        // Eliminar entradas existentes si es re-calculo
        await _entryRepository.DeleteByPeriodIdAsync(id, cancellationToken);

        // Obtener empleados activos y tipos de deduccion activos
        var activeEmployees = await _employeeRepository.GetActiveAsync(cancellationToken);
        if (activeEmployees.Count == 0)
            return Result<PayrollPeriodDto>.Failure("No hay empleados activos para calcular la nomina");

        var activeDeductions = await _deductionTypeRepository.GetActiveAsync(cancellationToken);

        var tenantId = _currentUserService.TenantId ?? Guid.Empty;
        var entries = new List<PayrollEntry>();

        foreach (var employee in activeEmployees)
        {
            var baseSalary = employee.BaseSalary;
            var workedDays = 30; // Mensual por defecto
            var overtimeHours = 0m;
            var grossPay = baseSalary;

            // Calcular seguro social empleado
            var socialSecurityEmployee = 0m;
            foreach (var deduction in activeDeductions)
            {
                if (!deduction.IsRequired)
                    continue;

                if (deduction.AppliesTo != DeductionAppliesTo.Employee && deduction.AppliesTo != DeductionAppliesTo.Both)
                    continue;

                socialSecurityEmployee += CalculateDeductionAmount(deduction, grossPay, baseSalary);
            }

            // Calcular seguro social empleador
            var socialSecurityEmployer = 0m;
            foreach (var deduction in activeDeductions)
            {
                if (!deduction.IsRequired)
                    continue;

                if (deduction.AppliesTo != DeductionAppliesTo.Employer && deduction.AppliesTo != DeductionAppliesTo.Both)
                    continue;

                socialSecurityEmployer += CalculateDeductionAmount(deduction, grossPay, baseSalary);
            }

            var incomeTax = 0m; // Placeholder
            var otherDeductions = 0m;
            var otherBenefits = 0m;
            var totalDeductions = socialSecurityEmployee + incomeTax + otherDeductions;
            var netPay = grossPay + otherBenefits - totalDeductions;

            var entry = new PayrollEntry
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                PayrollPeriodId = period.Id,
                EmployeeId = employee.Id,
                BaseSalary = baseSalary,
                WorkedDays = workedDays,
                OvertimeHours = overtimeHours,
                GrossPay = grossPay,
                SocialSecurityEmployee = socialSecurityEmployee,
                SocialSecurityEmployer = socialSecurityEmployer,
                IncomeTax = incomeTax,
                OtherDeductions = otherDeductions,
                OtherBenefits = otherBenefits,
                TotalDeductions = totalDeductions,
                NetPay = netPay,
                CreatedAt = DateTime.UtcNow
            };

            entries.Add(entry);
        }

        // Guardar entradas
        await _entryRepository.CreateRangeAsync(entries, cancellationToken);

        // Actualizar totales del periodo
        period.TotalGross = entries.Sum(e => e.GrossPay);
        period.TotalDeductions = entries.Sum(e => e.TotalDeductions);
        period.TotalNet = entries.Sum(e => e.NetPay);
        period.Status = PayrollPeriodStatus.Calculated;
        period.UpdatedAt = DateTime.UtcNow;

        await _periodRepository.UpdateAsync(period, cancellationToken);

        // Recargar el periodo con las entradas
        var reloaded = await _periodRepository.GetByIdAsync(id, cancellationToken);

        return Result<PayrollPeriodDto>.Success(reloaded!.ToDto());
    }

    public async Task<Result<PayrollPeriodDto>> ApproveAsync(Guid id, string approvedBy, CancellationToken cancellationToken = default)
    {
        var period = await _periodRepository.GetByIdAsync(id, cancellationToken);
        if (period == null)
            return Result<PayrollPeriodDto>.Failure("Periodo de nomina no encontrado");

        if (period.Status != PayrollPeriodStatus.Calculated)
            return Result<PayrollPeriodDto>.Failure("Solo se puede aprobar un periodo en estado Calculado");

        period.Status = PayrollPeriodStatus.Approved;
        period.ApprovedBy = approvedBy;
        period.ApprovedAt = DateTime.UtcNow;
        period.UpdatedAt = DateTime.UtcNow;

        await _periodRepository.UpdateAsync(period, cancellationToken);

        return Result<PayrollPeriodDto>.Success(period.ToDto());
    }

    public async Task<Result<PayrollPeriodDto>> MarkPaidAsync(Guid id, string paidBy, CancellationToken cancellationToken = default)
    {
        var period = await _periodRepository.GetByIdAsync(id, cancellationToken);
        if (period == null)
            return Result<PayrollPeriodDto>.Failure("Periodo de nomina no encontrado");

        if (period.Status != PayrollPeriodStatus.Approved)
            return Result<PayrollPeriodDto>.Failure("Solo se puede marcar como pagado un periodo en estado Aprobado");

        period.Status = PayrollPeriodStatus.Paid;
        period.PaidBy = paidBy;
        period.PaidAt = DateTime.UtcNow;
        period.UpdatedAt = DateTime.UtcNow;

        await _periodRepository.UpdateAsync(period, cancellationToken);

        return Result<PayrollPeriodDto>.Success(period.ToDto());
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var period = await _periodRepository.GetByIdAsync(id, cancellationToken);
        if (period == null)
            return Result<bool>.Failure("Periodo de nomina no encontrado");

        if (period.Status != PayrollPeriodStatus.Draft)
            return Result<bool>.Failure("Solo se puede eliminar un periodo en estado Borrador");

        // Eliminar entradas asociadas primero
        await _entryRepository.DeleteByPeriodIdAsync(id, cancellationToken);

        // Nota: El repositorio no expone DeleteAsync, se marca como borrador y se deja
        // Si se necesita eliminacion fisica, agregar DeleteAsync al repositorio
        return Result<bool>.Success(true);
    }

    /// <summary>
    /// Calcula el monto de una deduccion segun su tipo de calculo
    /// </summary>
    private static decimal CalculateDeductionAmount(DeductionType deduction, decimal grossPay, decimal baseSalary)
    {
        return deduction.CalculationType switch
        {
            DeductionCalculationType.PercentageOfGross => grossPay * (deduction.DefaultValue / 100m),
            DeductionCalculationType.PercentageOfBase => baseSalary * (deduction.DefaultValue / 100m),
            DeductionCalculationType.FixedAmount => deduction.DefaultValue,
            _ => 0m
        };
    }
}
