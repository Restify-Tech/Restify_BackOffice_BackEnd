namespace Restify.BackOffice.Application.DTOs;

public class SriReceptionResponse
{
    public string Status { get; set; } = string.Empty; // RECIBIDA, DEVUELTA
    public List<SriMessage> Messages { get; set; } = new();
}

public class SriAuthorizationResponse
{
    public string Status { get; set; } = string.Empty; // AUTORIZADO, NO AUTORIZADO
    public string? AuthorizationNumber { get; set; }
    public DateTime? AuthorizationDate { get; set; }
    public string? AuthorizedDocument { get; set; } // XML autorizado
    public List<SriMessage> Messages { get; set; } = new();
}

public class SriMessage
{
    public string Identifier { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? AdditionalInfo { get; set; }
    public string Type { get; set; } = string.Empty; // ERROR, ADVERTENCIA, INFORMATIVO
}
