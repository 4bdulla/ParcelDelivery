using AutoMapper;

using Core.Commands;
using Core.Common;
using Core.Dto;
using Core.Queries;

using MassTransit;

using ParcelApi.Data.Abstraction;
using ParcelApi.Data.Models;


namespace ParcelApi.Services;

/// <summary>
/// Handles parcel-related operations.
/// </summary>
public class ParcelApi(IParcelDbContext db, IMapper mapper, ILogger<ParcelApi> logger) :
    IConsumer<AssignParcelRequest>,
    IConsumer<CreateParcelRequest>,
    IConsumer<GetParcelRequest>,
    IConsumer<GetParcelsRequest>,
    IConsumer<GetParcelsByCourierRequest>,
    IConsumer<GetParcelsByUserRequest>,
    IConsumer<UpdateDestinationRequest>,
    IConsumer<UpdateParcelStatusRequest>,
    IConsumer<CancelParcelRequest>
{
    public async Task Consume(ConsumeContext<CreateParcelRequest> context)
    {
        Parcel parcel = new()
        {
            Status = ParcelStatus.Pending,
            UserId = context.Message.UserId,
            ParcelDetails = new(),
            DeliveryDetails = new()
            {
                SourceAddress = context.Message.SourceAddress,
                DestinationAddress = context.Message.DestinationAddress,
            }
        };

        await db.AddParcelAsync(parcel);

        await context.RespondAsync<CreateParcelResponse>(new() { Parcel = mapper.Map<ParcelDto>(parcel) });
    }

    public async Task Consume(ConsumeContext<GetParcelRequest> context)
    {
        Parcel parcel = await db.GetParcelByIdAsync(context.Message.ParcelId);

        await context.RespondAsync<GetParcelResponse>(new() { Parcel = mapper.Map<ParcelDto>(parcel) });
    }

    public async Task Consume(ConsumeContext<GetParcelsRequest> context)
    {
        IEnumerable<Parcel> parcels = await db.GetAllParcelsAsync();

        await context.RespondAsync<GetParcelsResponse>(new() { Parcels = mapper.Map<IEnumerable<ParcelDto>>(parcels) });
    }

    public async Task Consume(ConsumeContext<GetParcelsByUserRequest> context)
    {
        IEnumerable<Parcel> parcels = await db.GetAllParcelsAsync();

        await context.RespondAsync<GetParcelsByUserResponse>(new()
        {
            Parcels = mapper.Map<IEnumerable<ParcelDto>>(parcels.Where(p => p.UserId == context.Message.UserId))
        });
    }

    public async Task Consume(ConsumeContext<GetParcelsByCourierRequest> context)
    {
        IEnumerable<Parcel> parcels = await db.GetAllParcelsAsync();

        await context.RespondAsync<GetParcelsByCourierResponse>(new()
        {
            Parcels = mapper.Map<IEnumerable<ParcelDto>>(parcels.Where(p => p.CourierId == context.Message.CourierId))
        });
    }

    public async Task Consume(ConsumeContext<UpdateDestinationRequest> context)
    {
        Parcel parcel = await db.GetParcelByIdAsync(context.Message.ParcelId);

        if (parcel is null)
        {
            await context.RespondAsync<ParcelNotFoundResponse>(new() { ParcelId = context.Message.ParcelId });

            return;
        }

        parcel.DeliveryDetails.DestinationAddress = context.Message.NewDestination;

        await db.UpdateParcelAsync(parcel);

        await context.RespondAsync<UpdateDestinationResponse>(new() { Parcel = mapper.Map<ParcelDto>(parcel) });
    }

    public async Task Consume(ConsumeContext<UpdateParcelStatusRequest> context)
    {
        Parcel parcel = await db.GetParcelByIdAsync(context.Message.ParcelId);

        if (parcel is null)
        {
            await context.RespondAsync<ParcelNotFoundResponse>(new() { ParcelId = context.Message.ParcelId });

            return;
        }

        parcel.Status = context.Message.NewStatus;

        await db.UpdateParcelAsync(parcel);

        await context.RespondAsync<UpdateParcelStatusResponse>(new() { Parcel = mapper.Map<ParcelDto>(parcel) });
    }

    public async Task Consume(ConsumeContext<AssignParcelRequest> context)
    {
        Parcel parcel = await db.GetParcelByIdAsync(context.Message.ParcelId);

        if (parcel is null)
        {
            await context.RespondAsync<ParcelNotFoundResponse>(new() { ParcelId = context.Message.ParcelId });

            return;
        }

        parcel.CourierId = context.Message.CourierId;

        await db.UpdateParcelAsync(parcel);
        await context.RespondAsync<AssignParcelResponse>(new() { Parcel = mapper.Map<ParcelDto>(parcel) });
    }

    public async Task Consume(ConsumeContext<CancelParcelRequest> context)
    {
        Parcel parcel = await db.GetParcelByIdAsync(context.Message.ParcelId);

        parcel.Status = ParcelStatus.Canceled;

        await db.UpdateParcelAsync(parcel);

        await context.RespondAsync<CancelParcelResponse>(new() { Parcel = mapper.Map<ParcelDto>(parcel) });
    }
}