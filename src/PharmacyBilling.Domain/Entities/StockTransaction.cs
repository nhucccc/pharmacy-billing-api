using PharmacyBilling.Domain.Common;

namespace PharmacyBilling.Domain.Entities;

/// <summary>
/// Lịch sử giao dịch kho thuốc
/// </summary>
public class StockTransaction : BaseEntity
{
    public Guid MedicineId { get; private set; }
    public string TransactionType { get; private set; } = null!;  // IN, OUT, ADJUST_IN, ADJUST_OUT, RETURN
    public int Quantity { get; private set; }
    public int StockBefore { get; private set; }
    public int StockAfter { get; private set; }
    public decimal UnitPrice { get; private set; }
    public string? ReferenceId { get; private set; }    // Dispensation ID hoặc Import order ID
    public string? Note { get; private set; }
    public Guid? CreatedBy { get; private set; }

    // Navigation
    public Medicine Medicine { get; private set; } = null!;

    private StockTransaction() { }

    public static StockTransaction Create(
        Guid medicineId,
        string transactionType,
        int quantity,
        int stockBefore,
        int stockAfter,
        decimal unitPrice,
        string? referenceId = null,
        string? note = null,
        Guid? createdBy = null)
        => new StockTransaction
        {
            MedicineId = medicineId,
            TransactionType = transactionType,
            Quantity = quantity,
            StockBefore = stockBefore,
            StockAfter = stockAfter,
            UnitPrice = unitPrice,
            ReferenceId = referenceId,
            Note = note,
            CreatedBy = createdBy
        };
}
