using PharmacyBilling.Domain.Enums;
namespace PharmacyBilling.Application.DTOs.Invoice;

public record CreateInvoiceRequest(
    Guid PatientId,
    string PatientName,
    decimal ExaminationFee,
    string? PatientCode = null,
    string? InsuranceNumber = null,
    string? DoctorName = null,
    Guid? DispensationId = null,
    Guid? AppointmentId = null,
    decimal OtherFees = 0,
    decimal DiscountAmount = 0,
    decimal InsuranceCoverage = 0,
    string? Notes = null
);
