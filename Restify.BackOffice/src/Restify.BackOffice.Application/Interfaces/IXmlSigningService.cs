namespace Restify.BackOffice.Application.Interfaces;

public interface IXmlSigningService
{
    Task<string> SignXmlAsync(string xml, byte[] certificateData, string certificatePassword, CancellationToken ct = default);
}
