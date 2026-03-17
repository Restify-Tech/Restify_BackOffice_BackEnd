using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services.Gateways;

public class PaymentGatewayFactory : IPaymentGatewayFactory
{
    private readonly IEnumerable<IPaymentGateway> _gateways;

    public PaymentGatewayFactory(IEnumerable<IPaymentGateway> gateways)
    {
        _gateways = gateways;
    }

    public IPaymentGateway GetGateway(string gatewayName)
    {
        return _gateways.FirstOrDefault(g =>
            g.GatewayName.Equals(gatewayName, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException($"Gateway '{gatewayName}' no encontrado");
    }

    public IPaymentGateway GetDefaultGateway()
    {
        // Default to manual for now; later this comes from tenant config
        return _gateways.First(g => g.GatewayName == "Manual");
    }
}
