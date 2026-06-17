using PharmacyBilling.Domain.Common;

namespace PharmacyBilling.Domain.Events;

public sealed class DispensationCreatedEvent : BaseDomainEvent
{
    public Guid DispensationId { get; }
    public Guid PrescriptionId { get; }
    public Guid PatientId { get; }
    public override string EventType => "dispensation.created";

    public DispensationCreatedEvent(Guid dispensationId, Guid prescriptionId, Guid patientId)
    {
        DispensationId = dispensationId;
        PrescriptionId = prescriptionId;
        PatientId = patientId;
    }
}
