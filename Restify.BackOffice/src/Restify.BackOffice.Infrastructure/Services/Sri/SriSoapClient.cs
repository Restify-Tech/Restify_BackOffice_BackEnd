using System.Text;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Infrastructure.Services.Sri;

/// <summary>
/// Cliente SOAP para servicios web del SRI Ecuador.
/// Usa HttpClient directamente (sin WCF).
/// </summary>
public class SriSoapClient : ISriSoapClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SriSoapClient> _logger;

    private static readonly Dictionary<SriEnvironment, string> ReceptionUrls = new()
    {
        [SriEnvironment.Testing] = "https://celcer.sri.gob.ec/comprobantes-electronicos-ws/RecepcionComprobantesOffline?wsdl",
        [SriEnvironment.Production] = "https://cel.sri.gob.ec/comprobantes-electronicos-ws/RecepcionComprobantesOffline?wsdl"
    };

    private static readonly Dictionary<SriEnvironment, string> AuthorizationUrls = new()
    {
        [SriEnvironment.Testing] = "https://celcer.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantesOffline?wsdl",
        [SriEnvironment.Production] = "https://cel.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantesOffline?wsdl"
    };

    public SriSoapClient(IHttpClientFactory httpClientFactory, ILogger<SriSoapClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<SriReceptionResponse> SendDocumentAsync(string signedXml, SriEnvironment environment, CancellationToken ct = default)
    {
        var xmlBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(signedXml));

        var soapEnvelope = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ec=""http://ec.gob.sri.ws.recepcion"">
   <soapenv:Header/>
   <soapenv:Body>
      <ec:validarComprobante>
         <xml>{xmlBase64}</xml>
      </ec:validarComprobante>
   </soapenv:Body>
</soapenv:Envelope>";

        var url = ReceptionUrls[environment];
        var responseXml = await SendSoapRequestAsync(url, soapEnvelope, ct);

        return ParseReceptionResponse(responseXml);
    }

    public async Task<SriAuthorizationResponse> CheckAuthorizationAsync(string accessKey, SriEnvironment environment, CancellationToken ct = default)
    {
        var soapEnvelope = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ec=""http://ec.gob.sri.ws.autorizacion"">
   <soapenv:Header/>
   <soapenv:Body>
      <ec:autorizacionComprobante>
         <claveAccesoComprobante>{accessKey}</claveAccesoComprobante>
      </ec:autorizacionComprobante>
   </soapenv:Body>
</soapenv:Envelope>";

        var url = AuthorizationUrls[environment];
        var responseXml = await SendSoapRequestAsync(url, soapEnvelope, ct);

        return ParseAuthorizationResponse(responseXml);
    }

    private async Task<string> SendSoapRequestAsync(string url, string soapEnvelope, CancellationToken ct)
    {
        var client = _httpClientFactory.CreateClient("SRI");
        client.Timeout = TimeSpan.FromSeconds(30);

        var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");

        _logger.LogInformation("Enviando solicitud SOAP al SRI: {Url}", url);

        var response = await client.PostAsync(url, content, ct);
        var responseBody = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Error en respuesta SRI. Status: {Status}, Body: {Body}", response.StatusCode, responseBody);
            throw new HttpRequestException($"Error SRI HTTP {response.StatusCode}: {responseBody}");
        }

        return responseBody;
    }

    private static SriReceptionResponse ParseReceptionResponse(string responseXml)
    {
        var result = new SriReceptionResponse();

        try
        {
            var doc = XDocument.Parse(responseXml);
            var ns = doc.Root?.GetDefaultNamespace() ?? XNamespace.None;

            var estado = doc.Descendants()
                .FirstOrDefault(e => e.Name.LocalName == "estado");
            result.Status = estado?.Value ?? "DESCONOCIDO";

            var mensajes = doc.Descendants()
                .Where(e => e.Name.LocalName == "mensaje");

            foreach (var msg in mensajes)
            {
                result.Messages.Add(new SriMessage
                {
                    Identifier = msg.Descendants().FirstOrDefault(e => e.Name.LocalName == "identificador")?.Value ?? "",
                    Message = msg.Descendants().FirstOrDefault(e => e.Name.LocalName == "mensaje")?.Value ?? "",
                    AdditionalInfo = msg.Descendants().FirstOrDefault(e => e.Name.LocalName == "informacionAdicional")?.Value,
                    Type = msg.Descendants().FirstOrDefault(e => e.Name.LocalName == "tipo")?.Value ?? "ERROR"
                });
            }
        }
        catch (Exception)
        {
            result.Status = "ERROR_PARSE";
            result.Messages.Add(new SriMessage
            {
                Identifier = "PARSE",
                Message = "Error al parsear respuesta del SRI",
                Type = "ERROR"
            });
        }

        return result;
    }

    private static SriAuthorizationResponse ParseAuthorizationResponse(string responseXml)
    {
        var result = new SriAuthorizationResponse();

        try
        {
            var doc = XDocument.Parse(responseXml);

            var autorizacion = doc.Descendants()
                .FirstOrDefault(e => e.Name.LocalName == "autorizacion");

            if (autorizacion != null)
            {
                result.Status = autorizacion.Descendants()
                    .FirstOrDefault(e => e.Name.LocalName == "estado")?.Value ?? "DESCONOCIDO";

                result.AuthorizationNumber = autorizacion.Descendants()
                    .FirstOrDefault(e => e.Name.LocalName == "numeroAutorizacion")?.Value;

                var fechaStr = autorizacion.Descendants()
                    .FirstOrDefault(e => e.Name.LocalName == "fechaAutorizacion")?.Value;
                if (DateTime.TryParse(fechaStr, out var fecha))
                    result.AuthorizationDate = fecha;

                result.AuthorizedDocument = autorizacion.Descendants()
                    .FirstOrDefault(e => e.Name.LocalName == "comprobante")?.Value;

                var mensajes = autorizacion.Descendants()
                    .Where(e => e.Name.LocalName == "mensaje");

                foreach (var msg in mensajes)
                {
                    result.Messages.Add(new SriMessage
                    {
                        Identifier = msg.Descendants().FirstOrDefault(e => e.Name.LocalName == "identificador")?.Value ?? "",
                        Message = msg.Descendants().FirstOrDefault(e => e.Name.LocalName == "mensaje")?.Value ?? "",
                        AdditionalInfo = msg.Descendants().FirstOrDefault(e => e.Name.LocalName == "informacionAdicional")?.Value,
                        Type = msg.Descendants().FirstOrDefault(e => e.Name.LocalName == "tipo")?.Value ?? "ERROR"
                    });
                }
            }
            else
            {
                result.Status = "NO_ENCONTRADO";
            }
        }
        catch (Exception)
        {
            result.Status = "ERROR_PARSE";
            result.Messages.Add(new SriMessage
            {
                Identifier = "PARSE",
                Message = "Error al parsear respuesta de autorización del SRI",
                Type = "ERROR"
            });
        }

        return result;
    }
}
