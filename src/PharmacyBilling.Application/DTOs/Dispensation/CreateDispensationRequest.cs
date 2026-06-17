namespace PharmacyBilling.Application.DTOs.Dispensation;

public record CreateDispensationRequest(
    Guid PrescriptionId,
    Guid PatientId,
    Guid DoctorId,
    string PatientName,
    string DoctorName,
    string? Diagnosis,
    Guid? AppointmentId,
    string? Notes,
    List<CreateDispensationItemRequest> Items
);

public record CreateDispensationItemRequest(
    Guid MedicineId,
    int Quantity,
    string? Dosage,
    string? Usage,
    int? DurationDays
);
