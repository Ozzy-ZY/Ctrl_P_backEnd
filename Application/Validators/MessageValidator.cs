using Application.DTOs.Mappers;
using FluentValidation;

namespace Application.Validators;

public class MessageValidator : AbstractValidator<SendMessageDTO>
{
    public MessageValidator()
    {
        RuleFor(m => m.Subject).NotNull().WithMessage("Subject is required.")
            .NotEmpty().WithMessage("Message subject must not be empty")
            .MaximumLength(100).WithMessage("The max length of Message subject is 100 characters.");
        RuleFor(m => m.ContactEmail).EmailAddress();
        RuleFor(m => m.Content).NotNull().WithMessage("Content is requierd")
            .NotEmpty().WithMessage("Content must not be empty")
            .MaximumLength(500).WithMessage("The max length of Message Content is 500 characters.");
    }   
}