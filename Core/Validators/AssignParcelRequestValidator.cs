using Core.Commands.Parcel;

using FluentValidation;


namespace Core.Validators;

public class AssignParcelRequestValidator : AbstractValidator<AssignParcelRequest>
{
    public AssignParcelRequestValidator()
    {
        base.RuleFor(x => x.ParcelId).NotEmpty();
        base.RuleFor(x => x.CourierId).NotEmpty();
    }
}