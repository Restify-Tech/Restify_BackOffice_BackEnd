using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.BenefitHub;

/// <summary>
/// Cliente HTTP real para BenefitHub — se registra cuando BaseUrl y ApiKey estan configurados.
/// </summary>
public class HttpBenefitHubClient : IBenefitHubClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HttpBenefitHubClient> _logger;
    private readonly string _tenantSourceId;

    public HttpBenefitHubClient(
        HttpClient httpClient,
        ILogger<HttpBenefitHubClient> logger,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _tenantSourceId = configuration["BenefitHub:TenantSourceId"] ?? string.Empty;
    }

    public async Task<BenefitSimulationResult> SimulateAsync(BenefitRequest request)
    {
        try
        {
            var payload = new
            {
                tenantSourceId = request.TenantSourceId,
                externalCustomerId = request.ExternalCustomerId,
                externalTransactionId = request.ExternalTransactionId,
                items = request.Items.Select(i => new
                {
                    productId = i.ProductId,
                    productName = i.ProductName,
                    unitPrice = i.UnitPrice,
                    quantity = i.Quantity,
                    categoryId = i.CategoryId
                }),
                totalAmount = request.TotalAmount,
                couponCode = request.CouponCode
            };

            var response = await _httpClient.PostAsJsonAsync("/api/v1/benefits/simulate", payload);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("BenefitHub simulate retorno {StatusCode}", response.StatusCode);
                return new BenefitSimulationResult(false, new List<AppliedBenefit>(), 0, "BenefitHub no disponible");
            }

            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            var benefits = new List<AppliedBenefit>();
            decimal totalSaving = 0;

            if (result.TryGetProperty("data", out var data))
            {
                if (data.TryGetProperty("totalSaving", out var saving))
                    totalSaving = saving.GetDecimal();

                if (data.TryGetProperty("benefits", out var benefitsArray) && benefitsArray.ValueKind == JsonValueKind.Array)
                {
                    foreach (var b in benefitsArray.EnumerateArray())
                    {
                        benefits.Add(new AppliedBenefit(
                            b.TryGetProperty("benefitId", out var bid) ? bid.GetString() ?? "" : "",
                            b.TryGetProperty("name", out var bname) ? bname.GetString() ?? "" : "",
                            b.TryGetProperty("type", out var btype) ? btype.GetString() ?? "" : "",
                            b.TryGetProperty("amount", out var bamount) ? bamount.GetDecimal() : 0
                        ));
                    }
                }
            }

            return new BenefitSimulationResult(true, benefits, totalSaving);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "BenefitHub simulate fallo — usando resultado nulo");
            return new BenefitSimulationResult(false, new List<AppliedBenefit>(), 0, ex.Message);
        }
    }

    public async Task<BenefitApplyResult> ApplyAsync(BenefitRequest request)
    {
        try
        {
            var payload = new
            {
                tenantSourceId = request.TenantSourceId,
                externalCustomerId = request.ExternalCustomerId,
                externalTransactionId = request.ExternalTransactionId,
                items = request.Items.Select(i => new
                {
                    productId = i.ProductId,
                    productName = i.ProductName,
                    unitPrice = i.UnitPrice,
                    quantity = i.Quantity,
                    categoryId = i.CategoryId
                }),
                totalAmount = request.TotalAmount,
                couponCode = request.CouponCode
            };

            var response = await _httpClient.PostAsJsonAsync("/api/v1/benefits/apply", payload);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("BenefitHub apply retorno {StatusCode}", response.StatusCode);
                return new BenefitApplyResult(false, null, 0, 0, 0, null, "BenefitHub no disponible");
            }

            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            string? redemptionId = null;
            decimal totalSaving = 0;
            int pointsEarned = 0;
            int newPointsBalance = 0;
            string? membershipTier = null;

            if (result.TryGetProperty("data", out var data))
            {
                if (data.TryGetProperty("redemptionId", out var rid)) redemptionId = rid.GetString();
                if (data.TryGetProperty("totalSaving", out var saving)) totalSaving = saving.GetDecimal();
                if (data.TryGetProperty("pointsEarned", out var pe)) pointsEarned = pe.GetInt32();
                if (data.TryGetProperty("newPointsBalance", out var npb)) newPointsBalance = npb.GetInt32();
                if (data.TryGetProperty("membershipTier", out var tier)) membershipTier = tier.GetString();
            }

            return new BenefitApplyResult(true, redemptionId, totalSaving, pointsEarned, newPointsBalance, membershipTier);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "BenefitHub apply fallo — usando resultado nulo");
            return new BenefitApplyResult(false, null, 0, 0, 0, null, ex.Message);
        }
    }

    public async Task<bool> ReverseAsync(string externalTransactionId, string tenantSourceId, string? reason = null)
    {
        try
        {
            var payload = new
            {
                externalTransactionId,
                tenantSourceId,
                reason
            };

            var response = await _httpClient.PostAsJsonAsync("/api/v1/benefits/reverse", payload);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("BenefitHub reverse retorno {StatusCode} para transaccion {TransactionId}",
                    response.StatusCode, externalTransactionId);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "BenefitHub reverse fallo para transaccion {TransactionId}", externalTransactionId);
            return false;
        }
    }

    public async Task<CustomerBenefitsResult> GetCustomerBenefitsAsync(string externalCustomerId, string tenantSourceId)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"/api/v1/customers/{Uri.EscapeDataString(externalCustomerId)}/available?tenantSourceId={Uri.EscapeDataString(tenantSourceId)}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("BenefitHub getCustomerBenefits retorno {StatusCode} para cliente {CustomerId}",
                    response.StatusCode, externalCustomerId);
                return new CustomerBenefitsResult(false, 0, null, new List<AvailableBenefit>());
            }

            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            int points = 0;
            string? tier = null;
            var benefits = new List<AvailableBenefit>();

            if (result.TryGetProperty("data", out var data))
            {
                if (data.TryGetProperty("points", out var pts)) points = pts.GetInt32();
                if (data.TryGetProperty("tier", out var t)) tier = t.GetString();

                if (data.TryGetProperty("benefits", out var benefitsArray) && benefitsArray.ValueKind == JsonValueKind.Array)
                {
                    foreach (var b in benefitsArray.EnumerateArray())
                    {
                        benefits.Add(new AvailableBenefit(
                            b.TryGetProperty("id", out var bid) ? bid.GetString() ?? "" : "",
                            b.TryGetProperty("name", out var bname) ? bname.GetString() ?? "" : "",
                            b.TryGetProperty("type", out var btype) ? btype.GetString() ?? "" : "",
                            b.TryGetProperty("description", out var bdesc) ? bdesc.GetString() ?? "" : ""
                        ));
                    }
                }
            }

            return new CustomerBenefitsResult(true, points, tier, benefits);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "BenefitHub getCustomerBenefits fallo para cliente {CustomerId}", externalCustomerId);
            return new CustomerBenefitsResult(false, 0, null, new List<AvailableBenefit>());
        }
    }
}
