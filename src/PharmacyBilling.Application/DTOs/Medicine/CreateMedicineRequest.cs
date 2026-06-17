using PharmacyBilling.Domain.Enums;
namespace PharmacyBilling.Application.DTOs.Medicine;

public record CreateMedicineRequest(
    string Name,
    string ActiveIngredient,
    MedicineUnit Unit,
    decimal UnitPrice,
    decimal ImportPrice,
    int InitialStock,
    bool RequiresPrescription = false,
    string? Manufacturer = null,
    string? CountryOfOrigin = null,
    string? Category = null,
    string? Description = null,
    string? SideEffects = null,
    string? StorageConditions = null,
    string? UnitDescription = null,
    int MinimumStock = 10,
    DateTime? ExpiryDate = null
);
