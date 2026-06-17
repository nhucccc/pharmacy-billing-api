using PharmacyBilling.Domain.Common;

namespace PharmacyBilling.Domain.Entities;

public class DispensationItem : BaseEntity
{
    public Guid DispensationId { get; private set; }
    public Guid MedicineId { get; private set; }

    // Snapshot giá tại thời điểm xuất
    public string MedicineName { get; private set; } = null!;
    public string ActiveIngredient { get; private set; } = null!;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }  // Giá tại thời điểm xuất
    public decimal SubTotal => Quantity * UnitPrice;

    public string? Dosage { get; private set; }     // Liều dùng (VD: 1 viên/ngày, sáng chiều tối)
    public string? Usage { get; private set; }       // Cách dùng
    public int? DurationDays { get; private set; }  // Số ngày dùng

    // Navigation
    public Dispensation Dispensation { get; private set; } = null!;
    public Medicine Medicine { get; private set; } = null!;

    private DispensationItem() { }

    public static DispensationItem Create(
        Guid dispensationId,
        Guid medicineId,
        string medicineName,
        string activeIngredient,
        int quantity,
        decimal unitPrice,
        string? dosage = null,
        string? usage = null,
        int? durationDays = null)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive.");
        if (unitPrice < 0) throw new ArgumentException("Unit price cannot be negative.");

        return new DispensationItem
        {
            DispensationId = dispensationId,
            MedicineId = medicineId,
            MedicineName = medicineName,
            ActiveIngredient = activeIngredient,
            Quantity = quantity,
            UnitPrice = unitPrice,
            Dosage = dosage,
            Usage = usage,
            DurationDays = durationDays
        };
    }
}
