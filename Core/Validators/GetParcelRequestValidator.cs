using Core.Queries.Parcel;

using FluentValidation;


namespace Core.Validators;

public class GetParcelRequestValidator : AbstractValidator<GetParcelRequest>
{
    public GetParcelRequestValidator()
    {
        base.RuleFor(x => x.ParcelId).NotEmpty();
    }
}