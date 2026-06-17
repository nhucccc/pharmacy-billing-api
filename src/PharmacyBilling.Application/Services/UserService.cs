using PharmacyBilling.Application.Common.Models;
using PharmacyBilling.Application.DTOs.User;
using PharmacyBilling.Domain.Entities;
using PharmacyBilling.Domain.Enums;
using PharmacyBilling.Domain.Interfaces;

namespace PharmacyBilling.Application.Services;

public class UserService
{
    private readonly IUnitOfWork _uow;

    public UserService(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<PagedResult<UserDto>>> GetAllAsync(string? keyword, UserRole? role,
        bool? isActive, int page, int pageSize, CancellationToken ct = default)
    {
        var items = await _uow.Users.SearchAsync(keyword, role, isActive, page, pageSize, ct);
        var total = await _uow.Users.GetTotalCountAsync(keyword, role, isActive, ct);
        return Result<PagedResult<UserDto>>.Success(
            PagedResult<UserDto>.Create(items.Select(MapToDto).ToList(), total, page, pageSize));
    }

    public async Task<Result<UserDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByIdAsync(id, ct);
        if (user == null) return Result<UserDto>.NotFound("User not found.");
        return Result<UserDto>.Success(MapToDto(user));
    }

    public async Task<Result<UserDto>> UpdateProfileAsync(Guid id, UpdateProfileRequest request, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByIdAsync(id, ct);
        if (user == null) return Result<UserDto>.NotFound("User not found.");

        if (!await _uow.Users.IsEmailUniqueAsync(request.Email, id, ct))
            return Result<UserDto>.Conflict("Email already in use.");

        user.UpdateProfile(request.FullName, request.Email, request.PhoneNumber,
            request.Address, request.DateOfBirth, request.Gender, request.InsuranceNumber);
        _uow.Users.Update(user);
        await _uow.SaveChangesAsync(ct);
        return Result<UserDto>.Success(MapToDto(user));
    }

    public async Task<Result> ToggleActiveAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByIdAsync(id, ct);
        if (user == null) return Result.NotFound("User not found.");
        if (user.IsActive) user.Deactivate(); else user.Activate();
        _uow.Users.Update(user);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static UserDto MapToDto(User u) => new(
        u.Id, u.Username, u.FullName, u.Email, u.PhoneNumber,
        u.Role, u.Role.ToString(), u.IsActive,
        u.PatientCode, u.DateOfBirth, u.Gender, u.Address,
        u.InsuranceNumber, u.AvatarUrl, u.CreatedAt);
}
