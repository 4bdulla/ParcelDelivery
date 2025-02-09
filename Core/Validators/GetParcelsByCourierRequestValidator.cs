using Core.Queries.Parcel;

using FluentValidation;


namespace Core.Validators;

public class GetParcelsByCourierRequestValidator : AbstractValidator<GetParcelsByCourierRequest>
{
    public GetParcelsByCourierRequestValidator()
    {
        base.RuleFor(x => x.CourierId).NotEmpty();
    }
}