using PharmacyBilling.Domain.Common;
using PharmacyBilling.Domain.Enums;

namespace PharmacyBilling.Domain.Events;

public sealed class InvoicePaidEvent : BaseDomainEvent
{
    public Guid InvoiceId { get; }
    public Guid PatientId { get; }
    public decimal TotalAmount { get; }
    public PaymentMethod PaymentMethod { get; }
    public override string EventType => "invoice.paid";

    public InvoicePaidEvent(Guid invoiceId, Guid patientId, decimal totalAmount, PaymentMethod paymentMethod)
    {
        InvoiceId = invoiceId;
        PatientId = patientId;
        TotalAmount = totalAmount;
        PaymentMethod = paymentMethod;
    }
}
