namespace PharmacyBilling.Domain.Enums;

public enum InvoiceStatus
{
    Pending = 1,    // Chờ thanh toán
    Paid = 2,       // Đã thanh toán
    Cancelled = 3,  // Đã hủy
    Refunded = 4    // Đã hoàn tiền
}
