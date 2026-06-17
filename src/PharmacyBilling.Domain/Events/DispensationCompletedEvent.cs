using PharmacyBilling.Domain.Common;

namespace PharmacyBilling.Domain.Events;

public sealed class DispensationCompletedEvent : BaseDomainEvent
{
    public Guid DispensationId { get; }
    public Guid PatientId { get; }
    public decimal TotalMedicineCost { get; }
    public override string EventType => "dispensation.completed";

    public DispensationCompletedEvent(Guid dispensationId, Guid patientId, decimal totalMedicineCost)
    {
        DispensationId = dispensationId;
        PatientId = patientId;
        TotalMedicineCost = totalMedicineCost;
    }
}
