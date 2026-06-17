using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyBilling.Application.DTOs.Dispensation;
using PharmacyBilling.Application.Interfaces;
using PharmacyBilling.Application.Services;
using PharmacyBilling.Domain.Enums;

namespace PharmacyBilling.API.Controllers;

/// <summary>
/// Quản lý phiếu xuất thuốc - Dispensation Management
/// </summary>
[Tags("Dispensations")]
[Authorize]
public class DispensationsController : BaseController
{
    private readonly DispensationService _dispensationService;
    private readonly ICurrentUserService _currentUser;

    public DispensationsController(DispensationService dispensationService, ICurrentUserService currentUser)
    {
        _dispensationService = dispensationService;
        _currentUser = currentUser;
    }

    /// <summary>Get paginated dispensations with filters</summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Doctor,Nurse,Patient")]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? keyword,
        [FromQuery] DispensationStatus? status,
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

        return HandleResult(await _dispensationService.GetAllAsync(keyword, status, effectivePatientId, from, to, page, pageSize, ct));
    }

    /// <summary>Get dispensation by ID with items</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => HandleResult(await _dispensationService.GetByIdAsync(id, ct));

    /// <summary>Create dispensation from prescription (REST fallback - khi không dùng RabbitMQ)</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Nurse")]
    public async Task<IActionResult> Create([FromBody] CreateDispensationRequest request, CancellationToken ct)
        => HandleResult(await _dispensationService.CreateFromPrescriptionAsync(request, ct));

    /// <summary>Process dispensation - deduct stock and mark as dispensed</summary>
    [HttpPost("{id:guid}/process")]
    [Authorize(Roles = "Admin,Nurse")]
    public async Task<IActionResult> Process(Guid id, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? Guid.Empty;
        return HandleResult(await _dispensationService.ProcessDispensationAsync(id, userId, ct));
    }

    /// <summary>Cancel dispensation</summary>
    [HttpPost("{id:guid}/cancel")]
    [Authorize(Roles = "Admin,Nurse")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelRequest request, CancellationToken ct)
        => HandleResult(await _dispensationService.CancelAsync(id, request.Reason, ct));
}

public record CancelRequest(string Reason);
