using FluentValidation;
using PharmacyBilling.Application.DTOs.Auth;
using PharmacyBilling.Domain.Enums;

namespace PharmacyBilling.Application.Validators.Auth;

public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(3).MaximumLength(50)
            .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters, numbers, and underscores.");
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8)
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number.");
        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Passwords do not match.");
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(100);
        RuleFor(x => x.PhoneNumber).Matches(@"^[0-9]{10,11}$").When(x => x.PhoneNumber != null)
            .WithMessage("Phone number must be 10-11 digits.");
        RuleFor(x => x.Role).IsInEnum().WithMessage("Invalid role.");
    }
}
