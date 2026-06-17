using PharmacyBilling.Domain.Common;
using PharmacyBilling.Domain.Enums;
using PharmacyBilling.Domain.Events;

namespace PharmacyBilling.Domain.Entities;

/// <summary>
/// Phiếu xuất thuốc - được tạo khi nhận prescription.created event từ Medical Record Service
/// </summary>
public class Dispensation : BaseEntity
{
    public string DispensationCode { get; private set; } = null!;

    // Reference IDs từ các service khác (không FK cứng)
    public Guid PrescriptionId { get; private set; }    // ID từ Medical Record Service
    public Guid PatientId { get; private set; }          // ID từ User (hoặc Medical Record Service)
    public Guid DoctorId { get; private set; }           // ID từ Appointment Service
    public Guid? AppointmentId { get; private set; }     // ID từ Appointment Service

    // Thông tin snapshot tại thời điểm kê đơn
    public string PatientName { get; private set; } = null!;
    public string DoctorName { get; private set; } = null!;
    public string? Diagnosis { get; private set; }       // Chẩn đoán (snapshot từ medical record)
    public string? Notes { get; private set; }

    public DispensationStatus Status { get; private set; } = DispensationStatus.Pending;
    public DateTime? DispensedAt { get; private set; }
    public Guid? DispensedBy { get; private set; }       // Dược sĩ/Y tá xuất thuốc

    // Navigation
    public ICollection<DispensationItem> Items { get; private set; } = new List<DispensationItem>();
    public Invoice? Invoice { get; private set; }

    private Dispensation() { }

    public static Dispensation Create(
        Guid prescriptionId,
        Guid patientId,
        Guid doctorId,
        string patientName,
        string doctorName,
        string? diagnosis = null,
        Guid? appointmentId = null,
        string? notes = null)
    {
        var dispensation = new Dispensation
        {
            DispensationCode = GenerateCode(),
            PrescriptionId = prescriptionId,
            PatientId = patientId,
            DoctorId = doctorId,
            AppointmentId = appointmentId,
            PatientName = patientName.Trim(),
            DoctorName = doctorName.Trim(),
            Diagnosis = diagnosis,
            Notes = notes,
            Status = DispensationStatus.Pending
        };

        dispensation.AddDomainEvent(new DispensationCreatedEvent(dispensation.Id, prescriptionId, patientId));
        return dispensation;
    }

    public void AddItem(DispensationItem item)
    {
        if (Status != DispensationStatus.Pending && Status != DispensationStatus.Processing)
            throw new InvalidOperationException("Cannot add items to a dispensation that is not pending or processing.");
        Items.Add(item);
        SetUpdatedAt();
    }

    public void StartProcessing()
    {
        if (Status != DispensationStatus.Pending)
            throw new InvalidOperationException("Only pending dispensations can be processed.");
        Status = DispensationStatus.Processing;
        SetUpdatedAt();
    }

    public void Complete(Guid dispensedBy)
    {
        if (Status != DispensationStatus.Processing)
            throw new InvalidOperationException("Only processing dispensations can be completed.");
        Status = DispensationStatus.Dispensed;
        DispensedAt = DateTime.UtcNow;
        DispensedBy = dispensedBy;
        SetUpdatedAt();
        AddDomainEvent(new DispensationCompletedEvent(Id, PatientId, CalculateTotalMedicineCost()));
    }

    public void Cancel(string reason)
    {
        if (Status == DispensationStatus.Dispensed)
            throw new InvalidOperationException("Cannot cancel a completed dispensation.");
        Status = DispensationStatus.Cancelled;
        Notes = $"[CANCELLED] {reason}. {Notes}";
        SetUpdatedAt();
    }

    public decimal CalculateTotalMedicineCost()
        => Items.Sum(i => i.SubTotal);

    private static string GenerateCode()
        => $"DISP{DateTime.UtcNow:yyyyMMddHHmm}{Guid.NewGuid().ToString("N")[..4].ToUpper()}";
}
