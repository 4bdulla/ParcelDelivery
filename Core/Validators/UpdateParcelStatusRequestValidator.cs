using Core.Commands.Parcel;

using FluentValidation;


namespace Core.Validators;

public class UpdateParcelStatusRequestValidator : AbstractValidator<UpdateParcelStatusRequest>
{
    public UpdateParcelStatusRequestValidator()
    {
        base.RuleFor(x => x.ParcelId).NotEmpty();
        base.RuleFor(x => x.NewStatus).IsInEnum();
    }
}