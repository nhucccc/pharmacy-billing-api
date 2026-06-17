using PharmacyBilling.Domain.Enums;
namespace PharmacyBilling.Application.DTOs.Invoice;

public record InvoiceDto(
    Guid Id,
    string InvoiceCode,
    Guid PatientId,
    string PatientName,
    string? PatientCode,
    string? InsuranceNumber,
    string? DoctorName,
    Guid? DispensationId,
    Guid? AppointmentId,
    decimal ExaminationFee,
    decimal MedicineFee,
    decimal OtherFees,
    decimal DiscountAmount,
    decimal InsuranceCoverage,
    decimal TotalAmount,
    InvoiceStatus Status,
    string StatusName,
    PaymentMethod? PaymentMethod,
    string? PaymentMethodName,
    DateTime? PaidAt,
    string? Notes,
    List<InvoiceItemDto> Items,
    DateTime CreatedAt
);

public record InvoiceItemDto(
    Guid Id,
    string ItemName,
    string ItemType,
    int Quantity,
    decimal UnitPrice,
    decimal SubTotal,
    string? Note
);
