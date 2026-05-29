using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using Restify.BackOffice.Application.Interfaces;

#pragma warning disable SYSLIB0057 // X509Certificate2 constructor obsolete

namespace Restify.BackOffice.Infrastructure.Services.Sri;

/// <summary>
/// Firma digital XAdES-BES para comprobantes electronicos SRI Ecuador.
/// Construccion manual de la firma XML con control total sobre la estructura.
///
/// SignedXml de .NET no puede generar la estructura exacta que el SRI requiere:
/// - No resuelve id minuscula (XML usa Id)
/// - No resuelve forward references a DataObject children (#SignedProperties)
/// - No agrega Id al KeyInfo ni genera RSAKeyValue automaticamente
///
/// Esta implementacion construye cada componente como string XML,
/// computa digests con C14N + SHA1, y firma con RSA-SHA1.
///
/// Estructura seguida: Anexo 14, Ficha Tecnica SRI v2.32
/// </summary>
public class XAdESBESSigningService : IXmlSigningService
{
    private const string DsNs = "http://www.w3.org/2000/09/xmldsig#";
    private const string EtsiNs = "http://uri.etsi.org/01903/v1.3.2#";
    private const string C14NUrl = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";
    private const string RsaSha1Url = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
    private const string Sha1Url = "http://www.w3.org/2000/09/xmldsig#sha1";
    private const string EnvelopedSignatureUrl = "http://www.w3.org/2000/09/xmldsig#enveloped-signature";
    private const string SignedPropertiesType = "http://uri.etsi.org/01903#SignedProperties";

    public Task<string> SignXmlAsync(string xml, byte[] certificateData, string certificatePassword, CancellationToken ct = default)
    {
        // 1. Cargar certificado PKCS#12
        var cert = new X509Certificate2(certificateData, certificatePassword, X509KeyStorageFlags.Exportable);
        var privateKey = cert.GetRSAPrivateKey()
            ?? throw new CryptographicException("El certificado no contiene una clave privada RSA valida.");

        // Extraer parametros RSA (Modulus y Exponent)
        var rsaParams = privateKey.ExportParameters(false);
        var modulusBase64 = Convert.ToBase64String(rsaParams.Modulus!);
        var exponentBase64 = Convert.ToBase64String(rsaParams.Exponent!);
        var certBase64 = Convert.ToBase64String(cert.RawData);

        // 2. Generar IDs aleatorios de 6 digitos
        var id = GenerateId();

        var signatureId = $"Signature{id}";
        var signedPropertiesId = $"{signatureId}-SignedProperties";
        var certificateId = $"Certificate{id}";
        var signatureValueId = $"SignatureValue{id}";
        var referenceId = $"Reference-ID-{id}";
        var signedPropertiesRefId = $"SignedPropertiesID{id}";
        var objectId = $"{signatureId}-Object";

        // 3. Parsear XML del comprobante
        var xmlDoc = new XmlDocument { PreserveWhitespace = true };
        xmlDoc.LoadXml(xml);

        // 4. Construir KeyInfo XML
        var keyInfoXml = BuildKeyInfoXml(certBase64, modulusBase64, exponentBase64, certificateId);

        // 5. Construir QualifyingProperties XML
        var qualifyingPropsXml = BuildQualifyingPropertiesXml(cert, signatureId, signedPropertiesId, referenceId);

        // 6. Computar digests

        // 6a. Digest del documento (sin Signature)
        var docClone = (XmlDocument)xmlDoc.CloneNode(true);
        var existingSig = docClone.DocumentElement!.SelectSingleNode("*[local-name()='Signature']");
        if (existingSig != null)
            docClone.DocumentElement.RemoveChild(existingSig);
        var docDigest = ComputeSha1Base64(Canonicalize(docClone.DocumentElement!));

        // 6b. Digest de SignedProperties
        var spDoc = new XmlDocument { PreserveWhitespace = true };
        spDoc.LoadXml(qualifyingPropsXml);
        var nsm = new XmlNamespaceManager(spDoc.NameTable);
        nsm.AddNamespace("etsi", EtsiNs);
        var spNode = spDoc.SelectSingleNode("//etsi:SignedProperties", nsm)!;
        var spDigest = ComputeSha1Base64(Canonicalize(spNode));

        // 6c. Digest de KeyInfo
        var kiDoc = new XmlDocument { PreserveWhitespace = true };
        kiDoc.LoadXml(keyInfoXml);
        var kiDigest = ComputeSha1Base64(Canonicalize(kiDoc.DocumentElement!));

        // 7. Construir SignedInfo con 3 References
        var signedInfoXml = BuildSignedInfoXml(
            spDigest, kiDigest, docDigest,
            signedPropertiesRefId, signedPropertiesId,
            certificateId, referenceId);

        // 8. Canonicalizar SignedInfo y firmar con RSA-SHA1
        var siDoc = new XmlDocument { PreserveWhitespace = true };
        siDoc.LoadXml(signedInfoXml);
        var siCanonical = Canonicalize(siDoc.DocumentElement!);
        var signatureValue = Convert.ToBase64String(
            privateKey.SignData(siCanonical, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1));

        // 9. Ensamblar <ds:Signature> completo
        var signatureXml = $@"<ds:Signature xmlns:ds=""{DsNs}"" xmlns:etsi=""{EtsiNs}"" Id=""{signatureId}"">
{signedInfoXml}
<ds:SignatureValue Id=""{signatureValueId}"">{signatureValue}</ds:SignatureValue>
{keyInfoXml}
<ds:Object Id=""{objectId}"">
{qualifyingPropsXml}
</ds:Object>
</ds:Signature>";

        // 10. Parsear e insertar en el documento original
        var sigDoc = new XmlDocument { PreserveWhitespace = true };
        sigDoc.LoadXml(signatureXml);
        var importedNode = xmlDoc.ImportNode(sigDoc.DocumentElement!, true);
        xmlDoc.DocumentElement!.AppendChild(importedNode);

        // 11. Retornar XML firmado
        return Task.FromResult(xmlDoc.OuterXml);
    }

    private static string BuildKeyInfoXml(string certBase64, string modulusBase64, string exponentBase64, string certificateId)
    {
        return $@"<ds:KeyInfo xmlns:ds=""{DsNs}"" Id=""{certificateId}"">
<ds:X509Data>
<ds:X509Certificate>{certBase64}</ds:X509Certificate>
</ds:X509Data>
<ds:KeyValue>
<ds:RSAKeyValue>
<ds:Modulus>{modulusBase64}</ds:Modulus>
<ds:Exponent>{exponentBase64}</ds:Exponent>
</ds:RSAKeyValue>
</ds:KeyValue>
</ds:KeyInfo>";
    }

    private static string BuildQualifyingPropertiesXml(
        X509Certificate2 cert, string signatureId, string signedPropertiesId, string referenceId)
    {
        var certHash = SHA1.HashData(cert.RawData);
        var certHashBase64 = Convert.ToBase64String(certHash);

        var serialHex = cert.SerialNumber;
        var serialDecimal = BigInteger.Parse(serialHex, System.Globalization.NumberStyles.HexNumber).ToString();

        var signingTime = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
        var issuerDN = cert.Issuer;

        return $@"<etsi:QualifyingProperties xmlns:etsi=""{EtsiNs}"" Target=""#{signatureId}"">
<etsi:SignedProperties Id=""{signedPropertiesId}"">
<etsi:SignedSignatureProperties>
<etsi:SigningTime>{signingTime}</etsi:SigningTime>
<etsi:SigningCertificate>
<etsi:Cert>
<etsi:CertDigest>
<ds:DigestMethod xmlns:ds=""{DsNs}"" Algorithm=""{Sha1Url}""></ds:DigestMethod>
<ds:DigestValue xmlns:ds=""{DsNs}"">{certHashBase64}</ds:DigestValue>
</etsi:CertDigest>
<etsi:IssuerSerial>
<ds:X509IssuerName xmlns:ds=""{DsNs}"">{issuerDN}</ds:X509IssuerName>
<ds:X509SerialNumber xmlns:ds=""{DsNs}"">{serialDecimal}</ds:X509SerialNumber>
</etsi:IssuerSerial>
</etsi:Cert>
</etsi:SigningCertificate>
</etsi:SignedSignatureProperties>
<etsi:SignedDataObjectProperties>
<etsi:DataObjectFormat ObjectReference=""#{referenceId}"">
<etsi:Description>contenido comprobante</etsi:Description>
<etsi:MimeType>text/xml</etsi:MimeType>
</etsi:DataObjectFormat>
</etsi:SignedDataObjectProperties>
</etsi:SignedProperties>
</etsi:QualifyingProperties>";
    }

    private static string BuildSignedInfoXml(
        string spDigest, string kiDigest, string docDigest,
        string spRefId, string signedPropertiesId,
        string certificateId, string referenceId)
    {
        return $@"<ds:SignedInfo xmlns:ds=""{DsNs}"">
<ds:CanonicalizationMethod Algorithm=""{C14NUrl}""></ds:CanonicalizationMethod>
<ds:SignatureMethod Algorithm=""{RsaSha1Url}""></ds:SignatureMethod>
<ds:Reference Id=""{spRefId}"" Type=""{SignedPropertiesType}"" URI=""#{signedPropertiesId}"">
<ds:DigestMethod Algorithm=""{Sha1Url}""></ds:DigestMethod>
<ds:DigestValue>{spDigest}</ds:DigestValue>
</ds:Reference>
<ds:Reference URI=""#{certificateId}"">
<ds:DigestMethod Algorithm=""{Sha1Url}""></ds:DigestMethod>
<ds:DigestValue>{kiDigest}</ds:DigestValue>
</ds:Reference>
<ds:Reference Id=""{referenceId}"" URI=""#comprobante"">
<ds:Transforms>
<ds:Transform Algorithm=""{EnvelopedSignatureUrl}""></ds:Transform>
</ds:Transforms>
<ds:DigestMethod Algorithm=""{Sha1Url}""></ds:DigestMethod>
<ds:DigestValue>{docDigest}</ds:DigestValue>
</ds:Reference>
</ds:SignedInfo>";
    }

    private static byte[] Canonicalize(XmlNode node)
    {
        var doc = new XmlDocument { PreserveWhitespace = true };
        doc.LoadXml(node.OuterXml);
        var transform = new XmlDsigC14NTransform();
        transform.LoadInput(doc);
        using var stream = (Stream)transform.GetOutput(typeof(Stream));
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }

    private static string ComputeSha1Base64(byte[] data)
    {
        return Convert.ToBase64String(SHA1.HashData(data));
    }

    private static string GenerateId()
    {
        return Random.Shared.Next(100000, 999999).ToString();
    }
}
