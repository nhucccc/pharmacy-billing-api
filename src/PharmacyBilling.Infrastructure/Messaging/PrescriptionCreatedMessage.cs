namespace PharmacyBilling.Infrastructure.Messaging;

/// <summary>
/// Message nhận từ Medical Record Service khi bác sĩ kê đơn
/// </summary>
public class PrescriptionCreatedMessage
{
    public Guid PrescriptionId { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid? AppointmentId { get; set; }
    public string PatientName { get; set; } = null!;
    public string DoctorName { get; set; } = null!;
    public string? Diagnosis { get; set; }
    public string? Notes { get; set; }
    public List<PrescriptionItemMessage> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class PrescriptionItemMessage
{
    public Guid MedicineId { get; set; }
    public string MedicineName { get; set; } = null!;
    public int Quantity { get; set; }
    public string? Dosage { get; set; }
    public string? Usage { get; set; }
    public int? DurationDays { get; set; }
}
