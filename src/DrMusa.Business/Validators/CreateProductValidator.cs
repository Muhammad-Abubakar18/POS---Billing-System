using DrMusa.Business.DTOs;
using FluentValidation;

namespace DrMusa.Business.Validators;

public class CreateProductValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters.");

        RuleFor(x => x.Barcode)
            .MaximumLength(50).WithMessage("Barcode must not exceed 50 characters.")
            .When(x => !string.IsNullOrEmpty(x.Barcode));

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Please select a valid category.");

        RuleFor(x => x.PurchasePrice)
            .GreaterThanOrEqualTo(0).WithMessage("Purchase price must be 0 or greater.");

        RuleFor(x => x.SellingPrice)
            .GreaterThan(0).WithMessage("Selling price must be greater than 0.")
            .GreaterThanOrEqualTo(x => x.PurchasePrice)
                .WithMessage("Selling price must be >= purchase price.");
    }
}
