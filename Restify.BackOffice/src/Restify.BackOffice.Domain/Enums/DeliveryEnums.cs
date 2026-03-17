namespace Restify.BackOffice.Domain.Enums;

/// <summary>
/// Tipo de vehículo del repartidor
/// </summary>
public enum VehicleType
{
    Bicycle = 1,
    Motorcycle = 2,
    Car = 3,
    Van = 4
}

/// <summary>
/// Estado de verificación del repartidor pool
/// </summary>
public enum DriverVerificationStatus
{
    Pending = 1,
    UnderReview = 2,
    Approved = 3,
    Rejected = 4
}

/// <summary>
/// Tipo de documento del repartidor
/// </summary>
public enum DocumentType
{
    NationalId = 1,
    DriverLicense = 2,
    VehicleRegistration = 3,
    VehiclePhoto = 4,
    ProfilePhoto = 5
}

/// <summary>
/// Estado de un documento
/// </summary>
public enum DocumentStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3
}
