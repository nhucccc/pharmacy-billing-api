using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyBilling.Application.DTOs.User;
using PharmacyBilling.Application.Interfaces;
using PharmacyBilling.Application.Services;
using PharmacyBilling.Domain.Enums;

namespace PharmacyBilling.API.Controllers;

/// <summary>
/// Quản lý người dùng - User Management
/// </summary>
[Tags("Users")]
[Authorize]
public class UsersController : BaseController
{
    private readonly UserService _userService;
    private readonly ICurrentUserService _currentUser;

    public UsersController(UserService userService, ICurrentUserService currentUser)
    {
        _userService = userService;
        _currentUser = currentUser;
    }

    /// <summary>Get paginated users [Admin only]</summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? keyword,
        [FromQuery] UserRole? role,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
        => HandleResult(await _userService.GetAllAsync(keyword, role, isActive, page, pageSize, ct));

    /// <summary>Get user by ID</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => HandleResult(await _userService.GetByIdAsync(id, ct));

    /// <summary>Update my profile</summary>
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileRequest request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (!userId.HasValue) return Unauthorized();
        return HandleResult(await _userService.UpdateProfileAsync(userId.Value, request, ct));
    }

    /// <summary>Update user profile [Admin only]</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UpdateProfileRequest request, CancellationToken ct)
        => HandleResult(await _userService.UpdateProfileAsync(id, request, ct));

    /// <summary>Toggle user active/inactive [Admin only]</summary>
    [HttpPost("{id:guid}/toggle-active")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToggleActive(Guid id, CancellationToken ct)
        => HandleResult(await _userService.ToggleActiveAsync(id, ct));
}
