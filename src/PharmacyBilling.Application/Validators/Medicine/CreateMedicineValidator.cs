using FluentValidation;
using PharmacyBilling.Application.DTOs.Medicine;

namespace PharmacyBilling.Application.Validators.Medicine;

public class CreateMedicineValidator : AbstractValidator<CreateMedicineRequest>
{
    public CreateMedicineValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ActiveIngredient).NotEmpty().MaximumLength(300);
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ImportPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.InitialStock).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MinimumStock).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Unit).IsInEnum().WithMessage("Invalid unit.");
    }
}
