namespace ApiGateway;

using System.Net;
using Core.Commands;
using Core.Common;
using Core.Dto;
using Core.Queries;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

public static class Endpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapPost(nameof(CreateParcel), CreateParcel)
            .Produces<CreateParcelResponse>();

        app.MapGet(nameof(GetParcel), GetParcel).Produces<GetParcelResponse>();
        app.MapGet(nameof(GetParcels), GetParcels).Produces<GetParcelsResponse>();
        app.MapGet(nameof(GetParcelsByCourier), GetParcelsByCourier).Produces<GetParcelsByCourierResponse>();
        app.MapGet(nameof(GetParcelsByUser), GetParcelsByUser).Produces<GetParcelsByUserResponse>();

        app.MapPut(nameof(UpdateDestination), UpdateDestination)
            .Produces<UpdateDestinationResponse>()
            .Produces<ParcelNotFoundResponse>();

        app.MapPut(nameof(UpdateParcelStatus), UpdateParcelStatus)
            .Produces<UpdateParcelStatusResponse>()
            .Produces<ParcelNotFoundResponse>();

        app.MapPut(nameof(AssignParcel), AssignParcel)
            .Produces<AssignParcelResponse>()
            .Produces<ParcelNotFoundResponse>((int)HttpStatusCode.NotFound);

        app.MapDelete(nameof(CancelParcel), CancelParcel)
            .Produces<CancelParcelResponse>()
            .Produces<ParcelNotFoundResponse>();
    }

    /// <summary>
    /// Creates a new parcel.
    /// </summary>
    /// <param name="request">The request containing parcel details.</param>
    /// <returns>The result of the create parcel operation.</returns>
    private static async Task<IResult> CreateParcel(
        [FromBody] CreateParcelRequest request,
        [FromServices] IRequestClient<CreateParcelRequest> client,
        CancellationToken token)
    {
        Response<CreateParcelResponse> response = await client.GetResponse<CreateParcelResponse>(request, token);

        return Results.Ok(response.Message);
    }

    /// <summary>
    /// Gets a parcel by its ID.
    /// </summary>
    /// <param name="parcelId">The ID of the parcel.</param>
    /// <returns>The parcel details.</returns>
    private static async Task<GetParcelResponse> GetParcel(
        [FromQuery] int parcelId,
        [FromServices] IRequestClient<GetParcelRequest> client,
        CancellationToken token)
    {
        Response<GetParcelResponse> response = await client.GetResponse<GetParcelResponse>(new() { ParcelId = parcelId }, token);

        return response.Message;
    }

    /// <summary>
    /// Gets all parcels.
    /// </summary>
    /// <returns>The list of parcels.</returns>
    private static async Task<GetParcelsResponse> GetParcels(
        [FromServices] IRequestClient<GetParcelsRequest> client,
        CancellationToken token)
    {
        Response<GetParcelsResponse> response = await client.GetResponse<GetParcelsResponse>(new(), token);

        return response.Message;
    }

    /// <summary>
    /// Gets parcels by user ID.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>The list of parcels for the user.</returns>
    private static async Task<GetParcelsByUserResponse> GetParcelsByUser(
        [FromQuery] int userId,
        [FromServices] IRequestClient<GetParcelsByUserRequest> client,
        CancellationToken token)
    {
        Response<GetParcelsByUserResponse> response =
            await client.GetResponse<GetParcelsByUserResponse>(new() { UserId = userId }, token);

        return response.Message;
    }

    /// <summary>
    /// Gets parcels by courier ID.
    /// </summary>
    /// <param name="courierId">The ID of the courier.</param>
    /// <returns>The list of parcels for the courier.</returns>
    private static async Task<GetParcelsByCourierResponse> GetParcelsByCourier(
        [FromQuery] int courierId,
        [FromServices] IRequestClient<GetParcelsByCourierRequest> client,
        CancellationToken token)
    {
        Response<GetParcelsByCourierResponse> response =
            await client.GetResponse<GetParcelsByCourierResponse>(new() { CourierId = courierId }, token);

        return response.Message;
    }

    /// <summary>
    /// Updates the destination of a parcel.
    /// </summary>
    /// <param name="request">The request containing the new destination details.</param>
    /// <returns>The result of the update destination operation.</returns>
    private static async Task<object> UpdateDestination(
        [FromBody] UpdateDestinationRequest request,
        [FromServices] IRequestClient<UpdateDestinationRequest> client,
        CancellationToken token)
    {
        Response<UpdateDestinationResponse, ParcelNotFoundResponse> response =
            await client.GetResponse<UpdateDestinationResponse, ParcelNotFoundResponse>(request, token);

        return response.Message;
    }

    /// <summary>
    /// Updates the status of a parcel.
    /// </summary>
    /// <param name="request">The request containing the new status details.</param>
    /// <returns>The result of the update parcel status operation.</returns>
    private static async Task<object> UpdateParcelStatus(
        [FromBody] UpdateParcelStatusRequest request,
        [FromServices] IRequestClient<UpdateParcelStatusRequest> client,
        CancellationToken token)
    {
        Response<UpdateParcelStatusResponse, ParcelNotFoundResponse> response =
            await client.GetResponse<UpdateParcelStatusResponse, ParcelNotFoundResponse>(request, token);

        return response.Message;
    }

    /// <summary>
    /// Assigns a parcel to a courier.
    /// </summary>
    /// <param name="request">The request containing the assignment details.</param>
    /// <returns>The result of the assign parcel operation.</returns>
    private static async Task<IResult> AssignParcel(
        [FromBody] AssignParcelRequest request,
        [FromServices] IRequestClient<AssignParcelRequest> client,
        CancellationToken token)
    {
        Response<AssignParcelResponse, ParcelNotFoundResponse> response =
            await client.GetResponse<AssignParcelResponse, ParcelNotFoundResponse>(request, token);

        return response.Is<ParcelNotFoundResponse>(out Response<ParcelNotFoundResponse> result)
            ? Results.NotFound(result.Message)
            : Results.Ok(response.Message);
    }

    /// <summary>
    /// Cancels a parcel.
    /// </summary>
    /// <param name="request">The request containing the cancellation details.</param>
    /// <returns>The result of the cancel parcel operation.</returns>
    private static async Task<object> CancelParcel(
        [FromBody] CancelParcelRequest request,
        [FromServices] IRequestClient<CancelParcelRequest> client,
        CancellationToken token)
    {
        Response<CancelParcelResponse, ParcelNotFoundResponse> response =
            await client.GetResponse<CancelParcelResponse, ParcelNotFoundResponse>(request, token);

        return response.Message;
    }
}