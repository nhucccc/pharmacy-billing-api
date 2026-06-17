using PharmacyBilling.Application.Interfaces;

namespace PharmacyBilling.Infrastructure.Services;

public class PasswordService : IPasswordService
{
    public string HashPassword(string password)
        => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);

    public bool VerifyPassword(string password, string hash)
        => BCrypt.Net.BCrypt.Verify(password, hash);
}
