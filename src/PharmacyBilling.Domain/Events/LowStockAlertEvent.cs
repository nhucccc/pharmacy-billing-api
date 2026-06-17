using PharmacyBilling.Domain.Common;

namespace PharmacyBilling.Domain.Events;

public sealed class LowStockAlertEvent : BaseDomainEvent
{
    public Guid MedicineId { get; }
    public string MedicineName { get; }
    public int CurrentStock { get; }
    public int MinimumStock { get; }
    public override string EventType => "stock.low_alert";

    public LowStockAlertEvent(Guid medicineId, string medicineName, int currentStock, int minimumStock)
    {
        MedicineId = medicineId;
        MedicineName = medicineName;
        CurrentStock = currentStock;
        MinimumStock = minimumStock;
    }
}
