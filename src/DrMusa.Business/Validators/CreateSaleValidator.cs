using DrMusa.Business.DTOs;
using FluentValidation;

namespace DrMusa.Business.Validators;

public class CreateSaleValidator : AbstractValidator<CreateSaleDto>
{
    public CreateSaleValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("A sale must have at least one item.")
            .Must(items => items.All(i => i.Quantity > 0))
                .WithMessage("All item quantities must be greater than 0.")
            .Must(items => items.All(i => i.UnitPrice > 0))
                .WithMessage("All item unit prices must be greater than 0.");

        RuleFor(x => x.DiscountAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Discount amount cannot be negative.");

        RuleFor(x => x.TaxPercent)
            .InclusiveBetween(0, 100).WithMessage("Tax must be between 0 and 100%.");

        RuleFor(x => x.PaidAmount)
            .GreaterThan(0).WithMessage("Paid amount must be greater than 0.");
    }
}
