using Core.Commands.Parcel;

using FluentValidation;


namespace Core.Validators;

public class CreateParcelRequestValidator : AbstractValidator<CreateParcelRequest>
{
    public CreateParcelRequestValidator()
    {
        base.RuleFor(x => x.UserId).NotEmpty();
        base.RuleFor(x => x.SourceAddress).NotEmpty();
        base.RuleFor(x => x.DestinationAddress).NotEmpty();
    }
}