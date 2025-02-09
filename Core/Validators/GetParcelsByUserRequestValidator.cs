using Core.Queries.Parcel;

using FluentValidation;


namespace Core.Validators;

public class GetParcelsByUserRequestValidator : AbstractValidator<GetParcelsByUserRequest>
{
    public GetParcelsByUserRequestValidator()
    {
        base.RuleFor(x => x.UserId).NotEmpty();
    }
}