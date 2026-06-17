using PharmacyBilling.Domain.Common;

namespace PharmacyBilling.Domain.Events;

public sealed class InvoiceCreatedEvent : BaseDomainEvent
{
    public Guid InvoiceId { get; }
    public Guid PatientId { get; }
    public decimal TotalAmount { get; }
    public override string EventType => "invoice.created";

    public InvoiceCreatedEvent(Guid invoiceId, Guid patientId, decimal totalAmount)
    {
        InvoiceId = invoiceId;
        PatientId = patientId;
        TotalAmount = totalAmount;
    }
}
