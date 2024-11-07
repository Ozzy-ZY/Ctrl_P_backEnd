using Application.DTOs.AuthModels;
using FluentValidation;

namespace Application.Validators
{
    public class AppUserRegistrationDtoValidator : AbstractValidator<AppUserRegisterationDto>
    {
        public AppUserRegistrationDtoValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().Length(4, 64).WithMessage("Plese Enter a valid User Name (4, 64) characters");
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
            //RuleFor(x => x.Email).EmailAddress(mode: FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible); DataAnnotated
        }
    }
}