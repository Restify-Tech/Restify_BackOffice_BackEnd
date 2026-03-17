using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Interfaces;

public interface IRideGeneratorService
{
    byte[] GenerateInvoiceRide(Invoice invoice, ElectronicDocument document, FiscalConfiguration config);
    byte[] GenerateCreditNoteRide(CreditNote creditNote, ElectronicDocument document, FiscalConfiguration config);
    byte[] GenerateWithholdingRide(WithholdingVoucher voucher, ElectronicDocument document, FiscalConfiguration config);
}
