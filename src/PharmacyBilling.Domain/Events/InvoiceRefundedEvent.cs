using PharmacyBilling.Domain.Common;

namespace PharmacyBilling.Domain.Events;

public sealed class InvoiceRefundedEvent : BaseDomainEvent
{
    public Guid InvoiceId { get; }
    public Guid PatientId { get; }
    public decimal RefundAmount { get; }
    public override string EventType => "invoice.refunded";

    public InvoiceRefundedEvent(Guid invoiceId, Guid patientId, decimal refundAmount)
    {
        InvoiceId = invoiceId;
        PatientId = patientId;
        RefundAmount = refundAmount;
    }
}
