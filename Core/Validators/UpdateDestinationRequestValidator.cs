using Core.Commands.Parcel;

using FluentValidation;


namespace Core.Validators;

public class UpdateDestinationRequestValidator : AbstractValidator<UpdateDestinationRequest>
{
    public UpdateDestinationRequestValidator()
    {
        base.RuleFor(x => x.ParcelId).NotEmpty();
        base.RuleFor(x => x.NewDestination).NotEmpty();
    }
}