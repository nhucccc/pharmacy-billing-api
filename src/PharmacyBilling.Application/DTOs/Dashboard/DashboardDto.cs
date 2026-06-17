namespace PharmacyBilling.Application.DTOs.Dashboard;

public record DashboardDto(
    int TotalMedicines,
    int LowStockCount,
    int TodayDispensations,
    int PendingDispensations,
    decimal TodayRevenue,
    decimal MonthRevenue,
    int PendingInvoices,
    int TodayPaidInvoices,
    List<LowStockMedicineDto> LowStockMedicines,
    List<RevenueByDateDto> RevenueByDate
);

public record LowStockMedicineDto(Guid Id, string Name, string MedicineCode, int Stock, int MinimumStock);
public record RevenueByDateDto(string Date, decimal Amount);
