using Core.Commands.Auth;

using FluentValidation;


namespace Core.Validators;

public class UserRegistrationRequestValidator : AbstractValidator<UserRegistrationRequest>
{
    public UserRegistrationRequestValidator()
    {
        base.RuleFor(x => x.Username).NotEmpty();
        base.RuleFor(x => x.Password).NotEmpty();
    }
}