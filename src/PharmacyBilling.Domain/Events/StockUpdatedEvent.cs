using PharmacyBilling.Domain.Common;

namespace PharmacyBilling.Domain.Events;

public sealed class StockUpdatedEvent : BaseDomainEvent
{
    public Guid MedicineId { get; }
    public string MedicineName { get; }
    public int NewStock { get; }
    public int QuantityChanged { get; }
    public string TransactionType { get; }
    public string? Note { get; }
    public override string EventType => "stock.updated";

    public StockUpdatedEvent(Guid medicineId, string medicineName, int newStock,
        int quantityChanged, string transactionType, string? note)
    {
        MedicineId = medicineId;
        MedicineName = medicineName;
        NewStock = newStock;
        QuantityChanged = quantityChanged;
        TransactionType = transactionType;
        Note = note;
    }
}
