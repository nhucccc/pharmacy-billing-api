using PharmacyBilling.Domain.Common;

namespace PharmacyBilling.Domain.Events;

public sealed class MedicineCreatedEvent : BaseDomainEvent
{
    public Guid MedicineId { get; }
    public string MedicineName { get; }
    public int InitialStock { get; }
    public override string EventType => "medicine.created";

    public MedicineCreatedEvent(Guid medicineId, string medicineName, int initialStock)
    {
        MedicineId = medicineId;
        MedicineName = medicineName;
        InitialStock = initialStock;
    }
}
