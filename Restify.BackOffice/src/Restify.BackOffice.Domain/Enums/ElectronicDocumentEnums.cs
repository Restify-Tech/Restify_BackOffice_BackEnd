namespace Restify.BackOffice.Domain.Enums;

/// <summary>
/// Tipo de comprobante electronico segun SRI Ecuador
/// </summary>
public enum SriDocumentType
{
    /// <summary>Factura (01)</summary>
    Invoice = 1,
    /// <summary>Nota de Credito (04)</summary>
    CreditNote = 4,
    /// <summary>Comprobante de Retencion (07)</summary>
    WithholdingVoucher = 7
}

/// <summary>
/// Ambiente del SRI
/// </summary>
public enum SriEnvironment
{
    /// <summary>Pruebas (1)</summary>
    Testing = 1,
    /// <summary>Produccion (2)</summary>
    Production = 2
}

/// <summary>
/// Estado del documento electronico en el flujo SRI
/// </summary>
public enum ElectronicDocumentStatus
{
    /// <summary>Borrador, XML generado pero no firmado</summary>
    Draft = 1,
    /// <summary>XML firmado digitalmente</summary>
    Signed = 2,
    /// <summary>Enviado al SRI, esperando respuesta</summary>
    Sent = 3,
    /// <summary>Recibido por el SRI, pendiente de autorizacion</summary>
    Received = 4,
    /// <summary>Autorizado por el SRI</summary>
    Authorized = 5,
    /// <summary>Rechazado por el SRI</summary>
    Rejected = 6,
    /// <summary>Error en el procesamiento</summary>
    Error = 7
}

/// <summary>
/// Tipo de identificacion del comprador/proveedor segun SRI
/// </summary>
public enum SriIdentificationType
{
    /// <summary>RUC (04)</summary>
    Ruc = 4,
    /// <summary>Cedula (05)</summary>
    Cedula = 5,
    /// <summary>Pasaporte (06)</summary>
    Passport = 6,
    /// <summary>Consumidor Final (07)</summary>
    FinalConsumer = 7
}

/// <summary>
/// Forma de pago segun SRI
/// </summary>
public enum SriPaymentMethod
{
    /// <summary>Sin utilizacion del sistema financiero (01)</summary>
    Cash = 1,
    /// <summary>Tarjeta de debito (16)</summary>
    DebitCard = 16,
    /// <summary>Tarjeta de credito (19)</summary>
    CreditCard = 19,
    /// <summary>Otros con utilizacion del sistema financiero (20)</summary>
    Transfer = 20
}
