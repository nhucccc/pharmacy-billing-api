using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using PharmacyBilling.Application.Interfaces;
using PharmacyBilling.Domain.Enums;

namespace PharmacyBilling.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    public Guid? UserId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? Username
        => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

    public UserRole? Role
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
            return Enum.TryParse<UserRole>(value, out var role) ? role : null;
        }
    }

    public bool IsAuthenticated
        => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;
}
