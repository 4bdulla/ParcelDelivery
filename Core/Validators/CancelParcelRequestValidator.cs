using Core.Commands.Parcel;

using FluentValidation;


namespace Core.Validators;

public class CancelParcelRequestValidator : AbstractValidator<CancelParcelRequest>
{
    public CancelParcelRequestValidator()
    {
        base.RuleFor(x => x.ParcelId).NotEmpty();
    }
}