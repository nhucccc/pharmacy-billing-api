using PharmacyBilling.Domain.Enums;
namespace PharmacyBilling.Application.DTOs.Medicine;

public record MedicineDto(
    Guid Id,
    string MedicineCode,
    string Name,
    string ActiveIngredient,
    string? Manufacturer,
    string? CountryOfOrigin,
    MedicineUnit Unit,
    string UnitName,
    string? UnitDescription,
    string? Category,
    decimal UnitPrice,
    decimal ImportPrice,
    int StockQuantity,
    int MinimumStock,
    bool IsLowStock,
    string? Description,
    string? SideEffects,
    string? StorageConditions,
    bool RequiresPrescription,
    bool IsActive,
    DateTime? ExpiryDate,
    DateTime CreatedAt
);
