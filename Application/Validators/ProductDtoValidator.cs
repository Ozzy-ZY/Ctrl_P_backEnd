using Application.DTOs;
using FluentValidation;

namespace Application.Validators
{
    public class ProductDtoValidator : AbstractValidator<ProductDTO>
    {
        public ProductDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithSeverity(Severity.Error).WithMessage("Please, enter a product name.")
                .Length(4, 50).WithMessage("Please enter a name between 4 and 50 characters");

            RuleFor(x => x.Description)
                .Length(20, 400).WithMessage("Please enter a description between 20 and 400 characters")
                .NotEmpty().WithMessage("Please enter a description");

            RuleFor(x => x.OldPrice)
                .NotNull()
                .WithMessage("OldPrice is required when Sale is true.")
                .When(x => x.Sale);
        }

    }
}