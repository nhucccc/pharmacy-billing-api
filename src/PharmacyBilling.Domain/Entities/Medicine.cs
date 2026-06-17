using PharmacyBilling.Domain.Common;
using PharmacyBilling.Domain.Enums;
using PharmacyBilling.Domain.Events;

namespace PharmacyBilling.Domain.Entities;

public class Medicine : BaseEntity
{
    public string MedicineCode { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string ActiveIngredient { get; private set; } = null!;  // Hoạt chất
    public string? Manufacturer { get; private set; }               // Nhà sản xuất
    public string? CountryOfOrigin { get; private set; }
    public MedicineUnit Unit { get; private set; }
    public string? UnitDescription { get; private set; }            // Mô tả đơn vị (vd: hộp 10 vỉ x 10 viên)
    public string? Category { get; private set; }                   // Nhóm thuốc
    public decimal UnitPrice { get; private set; }                  // Giá bán lẻ
    public decimal ImportPrice { get; private set; }                // Giá nhập
    public int StockQuantity { get; private set; }                  // Tồn kho
    public int MinimumStock { get; private set; } = 10;            // Ngưỡng cảnh báo tồn kho thấp
    public string? Description { get; private set; }
    public string? SideEffects { get; private set; }                // Tác dụng phụ
    public string? StorageConditions { get; private set; }          // Điều kiện bảo quản
    public bool RequiresPrescription { get; private set; }          // Thuốc kê đơn
    public bool IsActive { get; private set; } = true;
    public DateTime? ExpiryDate { get; private set; }               // Hạn sử dụng lô thuốc hiện tại

    // Navigation
    public ICollection<DispensationItem> DispensationItems { get; private set; } = new List<DispensationItem>();
    public ICollection<StockTransaction> StockTransactions { get; private set; } = new List<StockTransaction>();

    private Medicine() { }

    public static Medicine Create(
        string name,
        string activeIngredient,
        MedicineUnit unit,
        decimal unitPrice,
        decimal importPrice,
        int initialStock,
        bool requiresPrescription = false,
        string? manufacturer = null,
        string? category = null,
        string? description = null,
        int minimumStock = 10)
    {
        if (unitPrice < 0) throw new InvalidOperationException("Unit price cannot be negative.");
        if (importPrice < 0) throw new InvalidOperationException("Import price cannot be negative.");
        if (initialStock < 0) throw new InvalidOperationException("Initial stock cannot be negative.");

        var medicine = new Medicine
        {
            MedicineCode = GenerateMedicineCode(),
            Name = name.Trim(),
            ActiveIngredient = activeIngredient.Trim(),
            Unit = unit,
            UnitPrice = unitPrice,
            ImportPrice = importPrice,
            StockQuantity = initialStock,
            RequiresPrescription = requiresPrescription,
            Manufacturer = manufacturer,
            Category = category,
            Description = description,
            MinimumStock = minimumStock
        };

        medicine.AddDomainEvent(new MedicineCreatedEvent(medicine.Id, medicine.Name, medicine.StockQuantity));
        return medicine;
    }

    public void UpdateInfo(string name, string activeIngredient, MedicineUnit unit,
        decimal unitPrice, decimal importPrice, string? manufacturer, string? category,
        string? description, string? sideEffects, string? storageConditions,
        bool requiresPrescription, int minimumStock)
    {
        Name = name.Trim();
        ActiveIngredient = activeIngredient.Trim();
        Unit = unit;
        UnitPrice = unitPrice;
        ImportPrice = importPrice;
        Manufacturer = manufacturer;
        Category = category;
        Description = description;
        SideEffects = sideEffects;
        StorageConditions = storageConditions;
        RequiresPrescription = requiresPrescription;
        MinimumStock = minimumStock;
        SetUpdatedAt();
    }

    public void AddStock(int quantity, string? note = null)
    {
        if (quantity <= 0) throw new InvalidOperationException("Quantity must be positive.");
        StockQuantity += quantity;
        SetUpdatedAt();
        AddDomainEvent(new StockUpdatedEvent(Id, Name, StockQuantity, quantity, "IN", note));
    }

    public void DeductStock(int quantity)
    {
        if (quantity <= 0) throw new InvalidOperationException("Quantity must be positive.");
        if (StockQuantity < quantity)
            throw new InvalidOperationException($"Insufficient stock. Available: {StockQuantity}, Requested: {quantity}");

        StockQuantity -= quantity;
        SetUpdatedAt();
        AddDomainEvent(new StockUpdatedEvent(Id, Name, StockQuantity, quantity, "OUT", null));

        if (StockQuantity <= MinimumStock)
            AddDomainEvent(new LowStockAlertEvent(Id, Name, StockQuantity, MinimumStock));
    }

    public void AdjustStock(int newQuantity, string reason)
    {
        if (newQuantity < 0) throw new InvalidOperationException("Stock quantity cannot be negative.");
        var diff = newQuantity - StockQuantity;
        StockQuantity = newQuantity;
        SetUpdatedAt();
        AddDomainEvent(new StockUpdatedEvent(Id, Name, StockQuantity, Math.Abs(diff), diff >= 0 ? "ADJUST_IN" : "ADJUST_OUT", reason));
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }

    public bool IsLowStock() => StockQuantity <= MinimumStock;

    private static string GenerateMedicineCode()
        => $"MED{DateTime.UtcNow:yyyyMMdd}{Guid.NewGuid().ToString("N")[..4].ToUpper()}";
}
