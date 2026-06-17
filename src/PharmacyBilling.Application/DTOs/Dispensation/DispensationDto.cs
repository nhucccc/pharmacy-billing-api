using PharmacyBilling.Domain.Enums;
namespace PharmacyBilling.Application.DTOs.Dispensation;

public record DispensationDto(
    Guid Id,
    string DispensationCode,
    Guid PrescriptionId,
    Guid PatientId,
    string PatientName,
    string DoctorName,
    string? Diagnosis,
    DispensationStatus Status,
    string StatusName,
    string? Notes,
    DateTime? DispensedAt,
    List<DispensationItemDto> Items,
    decimal TotalMedicineCost,
    DateTime CreatedAt
);

public record DispensationItemDto(
    Guid Id,
    Guid MedicineId,
    string MedicineName,
    string ActiveIngredient,
    int Quantity,
    decimal UnitPrice,
    decimal SubTotal,
    string? Dosage,
    string? Usage,
    int? DurationDays
);
