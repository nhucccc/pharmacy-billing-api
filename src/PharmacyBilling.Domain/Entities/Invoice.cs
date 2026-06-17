using PharmacyBilling.Domain.Common;
using PharmacyBilling.Domain.Enums;
using PharmacyBilling.Domain.Events;

namespace PharmacyBilling.Domain.Entities;

/// <summary>
/// Hóa đơn viện phí - gồm phí khám + tiền thuốc
/// </summary>
public class Invoice : BaseEntity
{
    public string InvoiceCode { get; private set; } = null!;

    // Reference IDs
    public Guid PatientId { get; private set; }
    public Guid? DispensationId { get; private set; }
    public Guid? AppointmentId { get; private set; }     // Để lấy phí khám

    // Thông tin snapshot
    public string PatientName { get; private set; } = null!;
    public string? PatientCode { get; private set; }
    public string? InsuranceNumber { get; private set; }
    public string? DoctorName { get; private set; }

    // Chi tiết tiền
    public decimal ExaminationFee { get; private set; }  // Phí khám từ Appointment Service
    public decimal MedicineFee { get; private set; }     // Tiền thuốc
    public decimal OtherFees { get; private set; } = 0;  // Phí khác (xét nghiệm, v.v.)
    public decimal DiscountAmount { get; private set; } = 0;
    public decimal InsuranceCoverage { get; private set; } = 0; // BHYT chi trả
    public decimal TotalAmount => ExaminationFee + MedicineFee + OtherFees - DiscountAmount - InsuranceCoverage;

    public InvoiceStatus Status { get; private set; } = InvoiceStatus.Pending;
    public PaymentMethod? PaymentMethod { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public Guid? CollectedBy { get; private set; }  // Thu ngân / Y tá
    public string? Notes { get; private set; }

    // Navigation
    public Dispensation? Dispensation { get; private set; }
    public ICollection<InvoiceItem> InvoiceItems { get; private set; } = new List<InvoiceItem>();

    private Invoice() { }

    public static Invoice Create(
        Guid patientId,
        string patientName,
        decimal examinationFee,
        decimal medicineFee,
        string? patientCode = null,
        string? insuranceNumber = null,
        string? doctorName = null,
        Guid? dispensationId = null,
        Guid? appointmentId = null,
        decimal otherFees = 0,
        string? notes = null)
    {
        if (examinationFee < 0 || medicineFee < 0 || otherFees < 0)
            throw new InvalidOperationException("Fees cannot be negative.");

        var invoice = new Invoice
        {
            InvoiceCode = GenerateCode(),
            PatientId = patientId,
            PatientName = patientName.Trim(),
            PatientCode = patientCode,
            InsuranceNumber = insuranceNumber,
            DoctorName = doctorName,
            DispensationId = dispensationId,
            AppointmentId = appointmentId,
            ExaminationFee = examinationFee,
            MedicineFee = medicineFee,
            OtherFees = otherFees,
            Notes = notes
        };

        invoice.AddDomainEvent(new InvoiceCreatedEvent(invoice.Id, patientId, invoice.TotalAmount));
        return invoice;
    }

    public void ApplyDiscount(decimal discountAmount, string reason)
    {
        if (discountAmount < 0) throw new ArgumentException("Discount cannot be negative.");
        if (discountAmount > ExaminationFee + MedicineFee + OtherFees)
            throw new InvalidOperationException("Discount cannot exceed total fees.");
        DiscountAmount = discountAmount;
        Notes = $"[DISCOUNT] {reason}. {Notes}";
        SetUpdatedAt();
    }

    public void ApplyInsurance(decimal coverageAmount)
    {
        if (coverageAmount < 0) throw new ArgumentException("Coverage cannot be negative.");
        InsuranceCoverage = coverageAmount;
        SetUpdatedAt();
    }

    public void MarkAsPaid(PaymentMethod paymentMethod, Guid collectedBy)
    {
        if (Status != InvoiceStatus.Pending)
            throw new InvalidOperationException("Only pending invoices can be paid.");
        if (TotalAmount < 0)
            throw new InvalidOperationException("Total amount cannot be negative after discounts.");

        Status = InvoiceStatus.Paid;
        PaymentMethod = paymentMethod;
        PaidAt = DateTime.UtcNow;
        CollectedBy = collectedBy;
        SetUpdatedAt();
        AddDomainEvent(new InvoicePaidEvent(Id, PatientId, TotalAmount, paymentMethod));
    }

    public void Cancel(string reason)
    {
        if (Status == InvoiceStatus.Paid)
            throw new InvalidOperationException("Cannot cancel a paid invoice. Use refund instead.");
        Status = InvoiceStatus.Cancelled;
        Notes = $"[CANCELLED] {reason}. {Notes}";
        SetUpdatedAt();
    }

    public void Refund(string reason)
    {
        if (Status != InvoiceStatus.Paid)
            throw new InvalidOperationException("Can only refund a paid invoice.");
        Status = InvoiceStatus.Refunded;
        Notes = $"[REFUNDED] {reason}. {Notes}";
        SetUpdatedAt();
        AddDomainEvent(new InvoiceRefundedEvent(Id, PatientId, TotalAmount));
    }

    private static string GenerateCode()
        => $"INV{DateTime.UtcNow:yyyyMMdd}{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
}
