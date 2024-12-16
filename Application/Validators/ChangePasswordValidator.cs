using Application.DTOs;
using FluentValidation;

namespace Application.Validators;

public class ChangePasswordValidator:AbstractValidator<ChangePasswordDTO>
{
    public ChangePasswordValidator()
    {
        // the Regex for the Identity validation
        RuleFor(p => p.NewPassword)
            .Matches("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$");
    } 
}