namespace PharmacyBilling.Domain.Enums;

public enum DispensationStatus
{
    Pending = 1,        // Chờ xuất thuốc
    Processing = 2,     // Đang xử lý
    Dispensed = 3,      // Đã xuất thuốc
    Cancelled = 4,      // Đã hủy
    PartiallyDispensed = 5 // Xuất một phần (thiếu hàng)
}
