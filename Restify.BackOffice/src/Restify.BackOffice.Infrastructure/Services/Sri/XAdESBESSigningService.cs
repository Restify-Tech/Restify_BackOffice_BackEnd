using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services.Sri;

/// <summary>
/// Firma digital XAdES-BES para comprobantes electronicos SRI Ecuador.
/// Usa APIs built-in de .NET: System.Security.Cryptography.Xml
/// </summary>
public class XAdESBESSigningService : IXmlSigningService
{
    private const string XAdESNamespace = "http://uri.etsi.org/01903/v1.3.2#";

    public Task<string> SignXmlAsync(string xml, byte[] certificateData, string certificatePassword, CancellationToken ct = default)
    {
        var cert = new X509Certificate2(certificateData, certificatePassword, X509KeyStorageFlags.Exportable);

        var xmlDoc = new XmlDocument { PreserveWhitespace = true };
        xmlDoc.LoadXml(xml);

        var signedXml = new SignedXml(xmlDoc)
        {
            SigningKey = cert.GetRSAPrivateKey()
        };

        // Referencia al nodo comprobante
        var reference = new Reference("#comprobante")
        {
            DigestMethod = SignedXml.XmlDsigSHA256Url
        };
        reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        reference.AddTransform(new XmlDsigExcC14NTransform());
        signedXml.AddReference(reference);

        // Referencia a SignedProperties (XAdES)
        var signedPropertiesId = "SignedProperties-" + Guid.NewGuid().ToString("N")[..8];
        var xadesReference = new Reference("#" + signedPropertiesId)
        {
            Type = "http://uri.etsi.org/01903#SignedProperties",
            DigestMethod = SignedXml.XmlDsigSHA256Url
        };
        xadesReference.AddTransform(new XmlDsigExcC14NTransform());
        signedXml.AddReference(xadesReference);

        // KeyInfo con X509Data
        var keyInfo = new KeyInfo();
        keyInfo.AddClause(new KeyInfoX509Data(cert));
        signedXml.KeyInfo = keyInfo;

        // Metodo de canonicalizacion y firma
        signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;
        signedXml.SignedInfo.SignatureMethod = SignedXml.XmlDsigRSASHA256Url;

        // Agregar XAdES SignedProperties como Object
        var signedProperties = CreateSignedProperties(cert, signedPropertiesId);
        var dataObject = new DataObject();
        dataObject.LoadXml(signedProperties);
        signedXml.AddObject(dataObject);

        // Computar firma
        signedXml.ComputeSignature();

        // Insertar firma en el documento
        var signatureNode = signedXml.GetXml();
        xmlDoc.DocumentElement!.AppendChild(xmlDoc.ImportNode(signatureNode, true));

        return Task.FromResult(xmlDoc.OuterXml);
    }

    private static XmlElement CreateSignedProperties(X509Certificate2 cert, string signedPropertiesId)
    {
        var certHash = SHA256.HashData(cert.RawData);
        var certHashBase64 = Convert.ToBase64String(certHash);

        var xmlDoc = new XmlDocument();
        var qualifyingProperties = xmlDoc.CreateElement("etsi", "QualifyingProperties", XAdESNamespace);
        qualifyingProperties.SetAttribute("Target", "#Signature");

        var signedProperties = xmlDoc.CreateElement("etsi", "SignedProperties", XAdESNamespace);
        signedProperties.SetAttribute("Id", signedPropertiesId);

        var signedSignatureProperties = xmlDoc.CreateElement("etsi", "SignedSignatureProperties", XAdESNamespace);

        // SigningTime
        var signingTime = xmlDoc.CreateElement("etsi", "SigningTime", XAdESNamespace);
        signingTime.InnerText = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        signedSignatureProperties.AppendChild(signingTime);

        // SigningCertificate
        var signingCertificate = xmlDoc.CreateElement("etsi", "SigningCertificate", XAdESNamespace);
        var certElement = xmlDoc.CreateElement("etsi", "Cert", XAdESNamespace);

        var certDigest = xmlDoc.CreateElement("etsi", "CertDigest", XAdESNamespace);
        var digestMethod = xmlDoc.CreateElement("ds", "DigestMethod", SignedXml.XmlDsigNamespaceUrl);
        digestMethod.SetAttribute("Algorithm", SignedXml.XmlDsigSHA256Url);
        var digestValue = xmlDoc.CreateElement("ds", "DigestValue", SignedXml.XmlDsigNamespaceUrl);
        digestValue.InnerText = certHashBase64;
        certDigest.AppendChild(digestMethod);
        certDigest.AppendChild(digestValue);
        certElement.AppendChild(certDigest);

        var issuerSerial = xmlDoc.CreateElement("etsi", "IssuerSerial", XAdESNamespace);
        var issuerName = xmlDoc.CreateElement("ds", "X509IssuerName", SignedXml.XmlDsigNamespaceUrl);
        issuerName.InnerText = cert.Issuer;
        var serialNumber = xmlDoc.CreateElement("ds", "X509SerialNumber", SignedXml.XmlDsigNamespaceUrl);
        serialNumber.InnerText = cert.SerialNumber;
        issuerSerial.AppendChild(issuerName);
        issuerSerial.AppendChild(serialNumber);
        certElement.AppendChild(issuerSerial);

        signingCertificate.AppendChild(certElement);
        signedSignatureProperties.AppendChild(signingCertificate);

        signedProperties.AppendChild(signedSignatureProperties);
        qualifyingProperties.AppendChild(signedProperties);
        xmlDoc.AppendChild(qualifyingProperties);

        return qualifyingProperties;
    }
}
