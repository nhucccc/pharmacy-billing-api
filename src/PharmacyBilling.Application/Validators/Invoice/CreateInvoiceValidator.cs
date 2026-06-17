using FluentValidation;
using PharmacyBilling.Application.DTOs.Invoice;

namespace PharmacyBilling.Application.Validators.Invoice;

public class CreateInvoiceValidator : AbstractValidator<CreateInvoiceRequest>
{
    public CreateInvoiceValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.PatientName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ExaminationFee).GreaterThanOrEqualTo(0);
        RuleFor(x => x.OtherFees).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DiscountAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.InsuranceCoverage).GreaterThanOrEqualTo(0);
    }
}
