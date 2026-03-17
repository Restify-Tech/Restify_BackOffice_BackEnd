using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Application.Interfaces;

public interface IAccessKeyGenerator
{
    /// <summary>
    /// Genera clave de acceso de 49 digitos para comprobantes electronicos SRI
    /// </summary>
    string Generate(DateTime date, SriDocumentType documentType, string ruc, SriEnvironment environment, string establishment, string emissionPoint, int sequential);
}
