using Core.Commands.Auth;

using FluentValidation;


namespace Core.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        base.RuleFor(x => x.Username).NotEmpty();
        base.RuleFor(x => x.Password).NotEmpty();
        base.RuleFor(x => x.Role).NotEmpty();
    }
}