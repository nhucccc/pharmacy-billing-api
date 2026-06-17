using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyBilling.Application.DTOs.Auth;
using PharmacyBilling.Application.Interfaces;
using PharmacyBilling.Application.Services;

namespace PharmacyBilling.API.Controllers;

/// <summary>
/// Authentication &amp; Authorization - JWT Token Management
/// </summary>
[Tags("Authentication")]
public class AuthController : BaseController
{
    private readonly AuthService _authService;
    private readonly ICurrentUserService _currentUser;

    public AuthController(AuthService authService, ICurrentUserService currentUser)
    {
        _authService = authService;
        _currentUser = currentUser;
    }

    /// <summary>Login - Get JWT Access Token + Refresh Token</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        => HandleResult(await _authService.LoginAsync(request, ct));

    /// <summary>Register new user account</summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
        => HandleResult(await _authService.RegisterAsync(request, ct));

    /// <summary>Register patient (public)</summary>
    [HttpPost("register/patient")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterPatient([FromBody] RegisterRequest request, CancellationToken ct)
    {
        // Force role to Patient for public registration
        var patientRequest = request with { Role = PharmacyBilling.Domain.Enums.UserRole.Patient };
        return HandleResult(await _authService.RegisterAsync(patientRequest, ct));
    }

    /// <summary>Refresh Access Token using Refresh Token</summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken ct)
        => HandleResult(await _authService.RefreshTokenAsync(request, ct));

    /// <summary>Logout - Revoke Refresh Token</summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (!userId.HasValue) return Unauthorized();
        return HandleResult(await _authService.LogoutAsync(userId.Value, ct));
    }

    /// <summary>Change password</summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (!userId.HasValue) return Unauthorized();
        return HandleResult(await _authService.ChangePasswordAsync(userId.Value, request, ct));
    }

    /// <summary>Get current user info (me)</summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me([FromServices] UserService userService, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (!userId.HasValue) return Unauthorized();
        return HandleResult(await userService.GetByIdAsync(userId.Value, ct));
    }
}
