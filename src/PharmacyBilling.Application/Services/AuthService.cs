using Microsoft.Extensions.Logging;
using PharmacyBilling.Application.DTOs.Auth;
using PharmacyBilling.Application.Interfaces;
using PharmacyBilling.Application.Common.Models;
using PharmacyBilling.Domain.Entities;
using PharmacyBilling.Domain.Enums;
using PharmacyBilling.Domain.Interfaces;

namespace PharmacyBilling.Application.Services;

public class AuthService
{
    private readonly IUnitOfWork _uow;
    private readonly IJwtService _jwt;
    private readonly IPasswordService _pwd;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUnitOfWork uow, IJwtService jwt, IPasswordService pwd, ILogger<AuthService> logger)
    {
        _uow = uow;
        _jwt = jwt;
        _pwd = pwd;
        _logger = logger;
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByUsernameAsync(request.Username.ToLower(), ct);
        if (user == null || !_pwd.VerifyPassword(request.Password, user.PasswordHash))
            return Result<AuthResponse>.Unauthorized("Invalid username or password.");

        if (!user.IsActive)
            return Result<AuthResponse>.Forbidden("Account is disabled. Please contact administrator.");

        var accessToken = _jwt.GenerateAccessToken(user);
        var refreshToken = _jwt.GenerateRefreshToken();
        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("User {Username} logged in successfully", user.Username);
        return Result<AuthResponse>.Success(MapToAuthResponse(user, accessToken, refreshToken));
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        if (!await _uow.Users.IsUsernameUniqueAsync(request.Username, null, ct))
            return Result<AuthResponse>.Conflict("Username already exists.");

        if (!await _uow.Users.IsEmailUniqueAsync(request.Email, null, ct))
            return Result<AuthResponse>.Conflict("Email already exists.");

        var hash = _pwd.HashPassword(request.Password);
        var user = User.Create(request.Username, hash, request.FullName, request.Email, request.Role, request.PhoneNumber);

        if (request.Role == UserRole.Patient)
            user.UpdateProfile(request.FullName, request.Email, request.PhoneNumber,
                request.Address, request.DateOfBirth, request.Gender, request.InsuranceNumber);

        await _uow.Users.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        var accessToken = _jwt.GenerateAccessToken(user);
        var refreshToken = _jwt.GenerateRefreshToken();
        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("New user registered: {Username} with role {Role}", user.Username, user.Role);
        return Result<AuthResponse>.Success(MapToAuthResponse(user, accessToken, refreshToken));
    }

    public async Task<Result<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct = default)
    {
        var (isValid, username) = _jwt.ValidateAccessToken(request.AccessToken);
        if (!isValid || username == null)
            return Result<AuthResponse>.Unauthorized("Invalid access token.");

        var user = await _uow.Users.GetByUsernameAsync(username, ct);
        if (user == null || user.RefreshToken != request.RefreshToken)
            return Result<AuthResponse>.Unauthorized("Invalid refresh token.");

        if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return Result<AuthResponse>.Unauthorized("Refresh token has expired.");

        var newAccessToken = _jwt.GenerateAccessToken(user);
        var newRefreshToken = _jwt.GenerateRefreshToken();
        user.SetRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));
        await _uow.SaveChangesAsync(ct);

        return Result<AuthResponse>.Success(MapToAuthResponse(user, newAccessToken, newRefreshToken));
    }

    public async Task<Result> LogoutAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByIdAsync(userId, ct);
        if (user == null) return Result.NotFound("User not found.");

        user.RevokeRefreshToken();
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByIdAsync(userId, ct);
        if (user == null) return Result.NotFound("User not found.");

        if (!_pwd.VerifyPassword(request.CurrentPassword, user.PasswordHash))
            return Result.Failure("Current password is incorrect.");

        user.ChangePassword(_pwd.HashPassword(request.NewPassword));
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    private AuthResponse MapToAuthResponse(User user, string accessToken, string refreshToken)
        => new(
            accessToken,
            refreshToken,
            _jwt.GetAccessTokenExpiry(),
            user.Id,
            user.Username,
            user.FullName,
            user.Email,
            user.Role,
            user.Role.ToString()
        );
}
