using Application.DTOs;
using FluentValidation;

namespace Application.Validators
{
    public class ProductDtoCreateValidator : AbstractValidator<ProductDTOCreate>
    {
        public ProductDtoCreateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithSeverity(Severity.Error).WithMessage("Please, enter a product name.")
                .Length(4, 50).WithMessage("Please enter a name between 4 and 50 characters");

            RuleFor(x => x.Description)
                .Length(20, 400).WithMessage("Please enter a description between 20 and 400 characters")
                .NotEmpty().WithMessage("Please enter a description");

            RuleFor(x => x)
                .Must(x => !x.OldPrice.HasValue || x.OldPrice > x.Price)
                .When(x => x.OldPrice.HasValue)
                .WithMessage("Old price must be greater than the current price.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Price cannot be negative.");
        }
    }
}