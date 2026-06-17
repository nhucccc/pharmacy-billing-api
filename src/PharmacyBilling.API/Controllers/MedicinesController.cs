using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyBilling.Application.DTOs.Medicine;
using PharmacyBilling.Application.Interfaces;
using PharmacyBilling.Application.Services;

namespace PharmacyBilling.API.Controllers;

/// <summary>
/// Quản lý kho thuốc - Medicine Inventory Management
/// </summary>
[Tags("Medicines")]
[Authorize]
public class MedicinesController : BaseController
{
    private readonly MedicineService _medicineService;
    private readonly ICurrentUserService _currentUser;

    public MedicinesController(MedicineService medicineService, ICurrentUserService currentUser)
    {
        _medicineService = medicineService;
        _currentUser = currentUser;
    }

    /// <summary>Get paginated list of medicines with search/filter</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? keyword,
        [FromQuery] string? category,
        [FromQuery] bool? activeOnly,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
        => HandleResult(await _medicineService.GetAllAsync(keyword, category, activeOnly, page, pageSize, ct));

    /// <summary>Get medicine by ID</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => HandleResult(await _medicineService.GetByIdAsync(id, ct));

    /// <summary>Get all medicine categories</summary>
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(CancellationToken ct)
        => HandleResult(await _medicineService.GetCategoriesAsync(ct));

    /// <summary>Get low stock medicines (below minimum threshold)</summary>
    [HttpGet("low-stock")]
    [Authorize(Roles = "Admin,Nurse")]
    public async Task<IActionResult> GetLowStock(CancellationToken ct)
        => HandleResult(await _medicineService.GetLowStockAsync(ct));

    /// <summary>Create new medicine [Admin only]</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateMedicineRequest request, CancellationToken ct)
        => HandleResult(await _medicineService.CreateAsync(request, ct));

    /// <summary>Update medicine info [Admin only]</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMedicineRequest request, CancellationToken ct)
        => HandleResult(await _medicineService.UpdateAsync(id, request, ct));

    /// <summary>Soft delete medicine [Admin only]</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => HandleResult(await _medicineService.DeleteAsync(id, ct));

    /// <summary>Import stock (nhập kho) [Admin, Nurse]</summary>
    [HttpPost("{id:guid}/import-stock")]
    [Authorize(Roles = "Admin,Nurse")]
    public async Task<IActionResult> ImportStock(Guid id, [FromBody] StockImportRequest request, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? Guid.Empty;
        return HandleResult(await _medicineService.ImportStockAsync(id, request, userId, ct));
    }

    /// <summary>Adjust stock (kiểm kê) [Admin only]</summary>
    [HttpPost("{id:guid}/adjust-stock")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdjustStock(Guid id, [FromBody] StockAdjustRequest request, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? Guid.Empty;
        return HandleResult(await _medicineService.AdjustStockAsync(id, request, userId, ct));
    }

    /// <summary>Get stock transaction history for a medicine</summary>
    [HttpGet("{id:guid}/stock-history")]
    public async Task<IActionResult> GetStockHistory(Guid id,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
        => HandleResult(await _medicineService.GetStockHistoryAsync(id, page, pageSize, ct));
}
