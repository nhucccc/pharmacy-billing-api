using FluentValidation;
using PharmacyBilling.Application.DTOs.Dispensation;

namespace PharmacyBilling.Application.Validators.Dispensation;

public class CreateDispensationValidator : AbstractValidator<CreateDispensationRequest>
{
    public CreateDispensationValidator()
    {
        RuleFor(x => x.PrescriptionId).NotEmpty();
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.DoctorId).NotEmpty();
        RuleFor(x => x.PatientName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DoctorName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Items).NotEmpty().WithMessage("Dispensation must have at least one item.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.MedicineId).NotEmpty();
            item.RuleFor(i => i.Quantity).GreaterThan(0);
        });
    }
}
