using FluentValidation;
using PharmacyBilling.Application.DTOs.Auth;

namespace PharmacyBilling.Application.Validators.Auth;

public class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(3).MaximumLength(50);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}
