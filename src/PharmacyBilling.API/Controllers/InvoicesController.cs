using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyBilling.Application.DTOs.Invoice;
using PharmacyBilling.Application.Interfaces;
using PharmacyBilling.Application.Services;
using PharmacyBilling.Domain.Enums;

namespace PharmacyBilling.API.Controllers;

/// <summary>
/// Quản lý hóa đơn viện phí - Invoice &amp; Billing Management
/// </summary>
[Tags("Invoices")]
[Authorize]
public class InvoicesController : BaseController
{
    private readonly InvoiceService _invoiceService;
    private readonly ICurrentUserService _currentUser;

    public InvoicesController(InvoiceService invoiceService, ICurrentUserService currentUser)
    {
        _invoiceService = invoiceService;
        _currentUser = currentUser;
    }

    /// <summary>Get paginated invoices with filters</summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Nurse,Patient")]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? keyword,
        [FromQuery] InvoiceStatus? status,
        [FromQuery] Guid? patientId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        // Bệnh nhân chỉ xem của mình
        var effectivePatientId = patientId;
        if (_currentUser.Role == UserRole.Patient)
            effectivePatientId = _currentUser.UserId;

        return HandleResult(await _invoiceService.GetAllAsync(keyword, status, effectivePatientId, from, to, page, pageSize, ct));
    }

    /// <summary>Get invoice by ID with items</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => HandleResult(await _invoiceService.GetByIdAsync(id, ct));

    /// <summary>Create invoice for patient</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Nurse")]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceRequest request, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? Guid.Empty;
        return HandleResult(await _invoiceService.CreateAsync(request, userId, ct));
    }

    /// <summary>Pay invoice (thu tiền)</summary>
    [HttpPost("{id:guid}/pay")]
    [Authorize(Roles = "Admin,Nurse")]
    public async Task<IActionResult> Pay(Guid id, [FromBody] PayInvoiceRequest request, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? Guid.Empty;
        return HandleResult(await _invoiceService.PayAsync(id, request, userId, ct));
    }

    /// <summary>Cancel invoice</summary>
    [HttpPost("{id:guid}/cancel")]
    [Authorize(Roles = "Admin,Nurse")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelInvoiceRequest request, CancellationToken ct)
        => HandleResult(await _invoiceService.CancelAsync(id, request.Reason, ct));

    /// <summary>Revenue report [Admin only]</summary>
    [HttpGet("reports/revenue")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RevenueReport(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken ct)
        => HandleResult(await _invoiceService.GetRevenueReportAsync(from, to, ct));
}

public record CancelInvoiceRequest(string Reason);
