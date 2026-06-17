using PharmacyBilling.Domain.Enums;
namespace PharmacyBilling.Application.DTOs.Medicine;

public record UpdateMedicineRequest(
    string Name,
    string ActiveIngredient,
    MedicineUnit Unit,
    decimal UnitPrice,
    decimal ImportPrice,
    string? Manufacturer,
    string? CountryOfOrigin,
    string? Category,
    string? Description,
    string? SideEffects,
    string? StorageConditions,
    string? UnitDescription,
    bool RequiresPrescription,
    int MinimumStock,
    DateTime? ExpiryDate
);
