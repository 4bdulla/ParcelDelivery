using AutoMapper;

using Core.Commands.Parcel;
using Core.Common.ErrorResponses;
using Core.Dto;
using Core.Queries.Parcel;

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

        logger.LogDebug("creating parcel {@Parcel}", parcel);

        await db.AddParcelAsync(parcel);

        logger.LogInformation("parcel created");

        await context.RespondAsync<CreateParcelResponse>(new() { Parcel = mapper.Map<ParcelDto>(parcel) });
    }

    public async Task Consume(ConsumeContext<GetParcelRequest> context)
    {
        Parcel parcel = await db.GetParcelByIdAsync(context.Message.ParcelId);

        logger.LogInformation("parcel found {@Parcel}", parcel);

        await context.RespondAsync<GetParcelResponse>(new() { Parcel = mapper.Map<ParcelDto>(parcel) });
    }

    public async Task Consume(ConsumeContext<GetParcelsRequest> context)
    {
        List<Parcel> parcels = await db.GetAllParcelsAsync();

        logger.LogInformation("parcel list fetched {Count}", parcels.Count);

        await context.RespondAsync<GetParcelsResponse>(new() { Parcels = mapper.Map<IEnumerable<ParcelDto>>(parcels) });
    }

    public async Task Consume(ConsumeContext<GetParcelsByUserRequest> context)
    {
        List<Parcel> parcels = await db.GetAllParcelsAsync();

        logger.LogInformation("parcel list fetched {Count}", parcels.Count);

        await context.RespondAsync<GetParcelsByUserResponse>(new()
        {
            Parcels = mapper.Map<IEnumerable<ParcelDto>>(parcels.Where(p => p.UserId == context.Message.UserId))
        });
    }

    public async Task Consume(ConsumeContext<GetParcelsByCourierRequest> context)
    {
        List<Parcel> parcels = await db.GetAllParcelsAsync();

        logger.LogInformation("parcel list fetched {Count}", parcels.Count);

        await context.RespondAsync<GetParcelsByCourierResponse>(new()
        {
            Parcels = mapper.Map<IEnumerable<ParcelDto>>(parcels.Where(p => p.CourierId == context.Message.CourierId))
        });
    }

    public async Task Consume(ConsumeContext<UpdateDestinationRequest> context)
    {
        Parcel parcel = await db.GetParcelByIdAsync(context.Message.ParcelId);

        logger.LogDebug("parcel found: {@Parcel}", parcel);

        if (parcel is null)
        {
            await context.RespondAsync<ParcelNotFoundResponse>(new() { ParcelId = context.Message.ParcelId });

            return;
        }

        if (parcel.Status is not ParcelStatus.Pending)
        {
            logger.LogWarning("attempt to update delivery address of not pending parcel");

            await context.RespondAsync<UpdateDestinationResponse>(new()
            {
                Parcel = mapper.Map<ParcelDto>(parcel),
                Updated = false
            });

            return;
        }

        string oldDestinationAddress = parcel.DeliveryDetails.DestinationAddress;

        parcel.DeliveryDetails.DestinationAddress = context.Message.NewDestination;

        logger.LogDebug("updating parcel destination address: from {OldAddress} to {NewAddress}",
            oldDestinationAddress,
            parcel.DeliveryDetails.DestinationAddress);

        await db.UpdateParcelAsync(parcel);

        logger.LogInformation("parcel destination address updated: {@Parcel}", parcel);

        await context.RespondAsync<UpdateDestinationResponse>(new()
        {
            Parcel = mapper.Map<ParcelDto>(parcel),
            Updated = true
        });
    }

    public async Task Consume(ConsumeContext<UpdateParcelStatusRequest> context)
    {
        Parcel parcel = await db.GetParcelByIdAsync(context.Message.ParcelId);

        logger.LogDebug("parcel found: {@Parcel}", parcel);

        if (parcel is null)
        {
            logger.LogWarning("parcel not found");

            await context.RespondAsync<ParcelNotFoundResponse>(new() { ParcelId = context.Message.ParcelId });

            return;
        }

        ParcelStatus oldStatus = parcel.Status;

        parcel.Status = context.Message.NewStatus;

        logger.LogDebug("updating parcel status: from {OldStatus} to {NewStatus}",
            oldStatus,
            parcel.Status);

        await db.UpdateParcelAsync(parcel);

        logger.LogInformation("parcel status updated: {@Parcel}", parcel);

        await context.RespondAsync<UpdateParcelStatusResponse>(new() { Parcel = mapper.Map<ParcelDto>(parcel) });
    }

    public async Task Consume(ConsumeContext<AssignParcelRequest> context)
    {
        Parcel parcel = await db.GetParcelByIdAsync(context.Message.ParcelId);

        logger.LogDebug("parcel found: {@Parcel}", parcel);

        if (parcel is null)
        {
            logger.LogWarning("parcel not found");

            await context.RespondAsync<ParcelNotFoundResponse>(new() { ParcelId = context.Message.ParcelId });

            return;
        }

        int? oldCourierId = parcel.CourierId;

        parcel.CourierId = context.Message.CourierId;

        logger.LogDebug("assigning parcel: from {OldCourierId} to {NewCourierId}",
            oldCourierId,
            parcel.Status);

        await db.UpdateParcelAsync(parcel);

        logger.LogInformation("parcel courier updated: {@Parcel}", parcel);

        await context.RespondAsync<AssignParcelResponse>(new() { Parcel = mapper.Map<ParcelDto>(parcel) });
    }

    public async Task Consume(ConsumeContext<CancelParcelRequest> context)
    {
        Parcel parcel = await db.GetParcelByIdAsync(context.Message.ParcelId);

        logger.LogDebug("parcel found: {@Parcel}", parcel);

        if (parcel is null)
        {
            logger.LogWarning("parcel not found");

            await context.RespondAsync<ParcelNotFoundResponse>(new() { ParcelId = context.Message.ParcelId });

            return;
        }

        if (parcel.Status is ParcelStatus.Assigned or ParcelStatus.Delivered or ParcelStatus.InProgress)
        {
            logger.LogWarning("attempt to cancel completed delivery {@Parcel}", parcel);

            await context.RespondAsync<CancelParcelResponse>(new()
            {
                Parcel = mapper.Map<ParcelDto>(parcel),
                Canceled = false
            });

            return;
        }

        parcel.Status = ParcelStatus.Canceled;

        await db.UpdateParcelAsync(parcel);

        logger.LogInformation("parcel canceled {@Parcel}", parcel);

        await context.RespondAsync<CancelParcelResponse>(new()
        {
            Parcel = mapper.Map<ParcelDto>(parcel),
            Canceled = true
        });
    }
}