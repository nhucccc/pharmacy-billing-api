using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyBilling.Domain.Enums;
using PharmacyBilling.Domain.Interfaces;

namespace PharmacyBilling.API.Controllers;

/// <summary>
/// Dashboard &amp; Statistics
/// </summary>
[Tags("Dashboard")]
[Authorize]
public class DashboardController : BaseController
{
    private readonly IUnitOfWork _uow;

    public DashboardController(IUnitOfWork uow) => _uow = uow;

    /// <summary>Get dashboard statistics</summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Nurse")]
    public async Task<IActionResult> GetStats(CancellationToken ct)
    {
        var totalMedicines = await _uow.Medicines.CountAsync(ct: ct);
        var lowStock = await _uow.Medicines.GetLowStockMedicinesAsync(ct);
        var todayDispensations = await _uow.Dispensations.GetTodayDispensationsAsync(ct);
        var pendingDispensations = await _uow.Dispensations.GetByStatusAsync(DispensationStatus.Pending, ct);
        var pendingInvoices = await _uow.Invoices.GetByStatusAsync(InvoiceStatus.Pending, ct);

        var today = DateTime.UtcNow.Date;
        var todayRevenue = await _uow.Invoices.GetTotalRevenueAsync(today, today.AddDays(1), ct);
        var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthRevenue = await _uow.Invoices.GetTotalRevenueAsync(monthStart, DateTime.UtcNow, ct);
        var revenueByDate = await _uow.Invoices.GetRevenueByDateAsync(
            DateTime.UtcNow.AddDays(-30), DateTime.UtcNow, ct);

        var stats = new
        {
            TotalMedicines = totalMedicines,
            LowStockCount = lowStock.Count,
            TodayDispensations = todayDispensations.Count,
            PendingDispensations = pendingDispensations.Count,
            PendingInvoices = pendingInvoices.Count,
            TodayRevenue = todayRevenue,
            MonthRevenue = monthRevenue,
            LowStockMedicines = lowStock.Take(10).Select(m => new
            {
                m.Id, m.Name, m.MedicineCode, Stock = m.StockQuantity, m.MinimumStock
            }),
            RevenueByDate = revenueByDate.Select(kv => new { Date = kv.Key, Amount = kv.Value })
        };

        return Ok(new { success = true, data = stats });
    }
}
