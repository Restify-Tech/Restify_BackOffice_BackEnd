using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Infrastructure.Services.Sri;

/// <summary>
/// Genera clave de acceso de 49 digitos para comprobantes electronicos SRI Ecuador.
/// Estructura: fecha(8) + tipoDoc(2) + RUC(13) + ambiente(1) + serie(6) + secuencial(9) + codigo(8) + tipoEmision(1) + digitoVerificador(1)
/// </summary>
public class AccessKeyGenerator : IAccessKeyGenerator
{
    public string Generate(DateTime date, SriDocumentType documentType, string ruc, SriEnvironment environment, string establishment, string emissionPoint, int sequential)
    {
        var dateStr = date.ToString("ddMMyyyy");
        var docTypeStr = ((int)documentType).ToString("D2");
        var envStr = ((int)environment).ToString();
        var serie = establishment + emissionPoint;
        var seqStr = sequential.ToString("D9");
        var numericCode = GenerateNumericCode();
        var emissionType = "1"; // Emision normal

        var baseKey = dateStr + docTypeStr + ruc + envStr + serie + seqStr + numericCode + emissionType;

        var checkDigit = CalculateModulo11(baseKey);

        return baseKey + checkDigit;
    }

    private static string GenerateNumericCode()
    {
        return Random.Shared.Next(10000000, 99999999).ToString();
    }

    /// <summary>
    /// Calcula digito verificador modulo 11 con factores 2-7
    /// </summary>
    internal static int CalculateModulo11(string value)
    {
        var factors = new[] { 2, 3, 4, 5, 6, 7 };
        var sum = 0;

        for (var i = value.Length - 1; i >= 0; i--)
        {
            var digit = value[i] - '0';
            var factorIndex = (value.Length - 1 - i) % factors.Length;
            sum += digit * factors[factorIndex];
        }

        var remainder = sum % 11;
        var result = 11 - remainder;

        return result switch
        {
            11 => 0,
            10 => 1,
            _ => result
        };
    }
}
