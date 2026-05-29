using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.BenefitHub;

/// <summary>
/// Implementacion Null Object — se usa cuando BenefitHub no esta configurado.
/// Retorna resultados vacios sin lanzar excepciones.
/// </summary>
public class NullBenefitHubClient : IBenefitHubClient
{
    public Task<BenefitSimulationResult> SimulateAsync(BenefitRequest request) =>
        Task.FromResult(new BenefitSimulationResult(true, new List<AppliedBenefit>(), 0));

    public Task<BenefitApplyResult> ApplyAsync(BenefitRequest request) =>
        Task.FromResult(new BenefitApplyResult(true, null, 0, 0, 0, null));

    public Task<bool> ReverseAsync(string externalTransactionId, string tenantSourceId, string? reason = null) =>
        Task.FromResult(true);

    public Task<CustomerBenefitsResult> GetCustomerBenefitsAsync(string externalCustomerId, string tenantSourceId) =>
        Task.FromResult(new CustomerBenefitsResult(true, 0, null, new List<AvailableBenefit>()));
}
