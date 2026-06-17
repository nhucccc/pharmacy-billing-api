using PharmacyBilling.Domain.Enums;
namespace PharmacyBilling.Application.DTOs.Invoice;
public record PayInvoiceRequest(PaymentMethod PaymentMethod);
