using Core.Commands.Auth;

using FluentValidation;


namespace Core.Validators;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        base.RuleFor(x => x.Username).NotEmpty();
        base.RuleFor(x => x.RefreshToken).NotEmpty();
        base.RuleFor(x => x.Role).NotEmpty();
    }
}