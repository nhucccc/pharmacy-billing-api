using PharmacyBilling.Domain.Common;
using PharmacyBilling.Domain.Enums;
using PharmacyBilling.Domain.Events;

namespace PharmacyBilling.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string FullName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string? PhoneNumber { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; } = true;
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiryTime { get; private set; }
    public string? AvatarUrl { get; private set; }

    // For Patient
    public string? PatientCode { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public string? Gender { get; private set; }
    public string? Address { get; private set; }
    public string? InsuranceNumber { get; private set; }

    private User() { }

    public static User Create(
        string username,
        string passwordHash,
        string fullName,
        string email,
        UserRole role,
        string? phoneNumber = null)
    {
        var user = new User
        {
            Username = username.Trim().ToLower(),
            PasswordHash = passwordHash,
            FullName = fullName.Trim(),
            Email = email.Trim().ToLower(),
            Role = role,
            PhoneNumber = phoneNumber
        };

        if (role == UserRole.Patient)
            user.PatientCode = GeneratePatientCode();

        user.AddDomainEvent(new UserCreatedEvent(user.Id, user.Username, user.Role));
        return user;
    }

    public void UpdateProfile(string fullName, string email, string? phoneNumber, string? address,
        DateTime? dateOfBirth, string? gender, string? insuranceNumber)
    {
        FullName = fullName.Trim();
        Email = email.Trim().ToLower();
        PhoneNumber = phoneNumber;
        Address = address;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        InsuranceNumber = insuranceNumber;
        SetUpdatedAt();
    }

    public void SetRefreshToken(string refreshToken, DateTime expiryTime)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = expiryTime;
        SetUpdatedAt();
    }

    public void RevokeRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiryTime = null;
        SetUpdatedAt();
    }

    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        RevokeRefreshToken();
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        IsActive = false;
        RevokeRefreshToken();
        SetUpdatedAt();
    }

    public void Activate()
    {
        IsActive = true;
        SetUpdatedAt();
    }

    public void SetAvatar(string avatarUrl)
    {
        AvatarUrl = avatarUrl;
        SetUpdatedAt();
    }

    private static string GeneratePatientCode()
        => $"BN{DateTime.UtcNow:yyyyMMdd}{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
}
