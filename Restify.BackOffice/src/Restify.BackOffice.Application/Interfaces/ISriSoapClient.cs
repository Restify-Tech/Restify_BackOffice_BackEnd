using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Application.Interfaces;

public interface ISriSoapClient
{
    Task<SriReceptionResponse> SendDocumentAsync(string signedXml, SriEnvironment environment, CancellationToken ct = default);
    Task<SriAuthorizationResponse> CheckAuthorizationAsync(string accessKey, SriEnvironment environment, CancellationToken ct = default);
}
