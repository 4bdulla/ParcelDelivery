using Core.Commands.Auth;
using Core.Commands.Parcel;
using Core.Common;
using Core.Common.ErrorResponses;
using Core.Queries.Parcel;

using Microsoft.OpenApi.Models;


namespace ApiGateway;

using System.Net;

using MassTransit;

using Microsoft.AspNetCore.Mvc;


public static class Endpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        RouteGroupBuilder authGroup = app.MapGroup("auth").WithTags("Authorization");

        authGroup.MapPost(nameof(RegisterUser), RegisterUser)
            .Produces<AuthorizationResponse>()
            .ProducesProblem((int)HttpStatusCode.Unauthorized);

        authGroup.MapPost(nameof(RegisterCourier), RegisterCourier)
            .Produces<AuthorizationResponse>()
            .ProducesProblem((int)HttpStatusCode.Unauthorized)
            .RequireRoleBasedAuthorization(Constants.AdminRole);

        authGroup.MapPost(nameof(Login), Login)
            .Produces<AuthorizationResponse>()
            .Produces<UnauthorizedResponse>((int)HttpStatusCode.Unauthorized);

        authGroup.MapPost(nameof(RefreshToken), RefreshToken)
            .Produces<AuthorizationResponse>()
            .Produces<UnauthorizedResponse>((int)HttpStatusCode.Unauthorized);

        RouteGroupBuilder parcelGroup = app.MapGroup("/parcel").WithTags("Parcel");

        parcelGroup.MapPost(nameof(CreateParcel), CreateParcel)
            .Produces<CreateParcelResponse>()
            .RequireRoleBasedAuthorization(Constants.AdminRole, Constants.UserRole);

        parcelGroup.MapGet(nameof(GetParcel), GetParcel)
            .Produces<GetParcelResponse>()
            .RequireRoleBasedAuthorization(Constants.AdminRole, Constants.UserRole, Constants.CourierRole);

        parcelGroup.MapGet(nameof(GetParcels), GetParcels)
            .Produces<GetParcelsResponse>()
            .RequireRoleBasedAuthorization(Constants.AdminRole);

        parcelGroup.MapGet(nameof(GetParcelsByCourier), GetParcelsByCourier)
            .Produces<GetParcelsByCourierResponse>()
            .RequireRoleBasedAuthorization(Constants.AdminRole, Constants.CourierRole);

        parcelGroup.MapGet(nameof(GetParcelsByUser), GetParcelsByUser)
            .Produces<GetParcelsByUserResponse>()
            .RequireRoleBasedAuthorization(Constants.AdminRole, Constants.UserRole);

        parcelGroup.MapPut(nameof(UpdateDestination), UpdateDestination)
            .Produces<UpdateDestinationResponse>()
            .Produces<ParcelNotFoundResponse>((int)HttpStatusCode.NotFound)
            .RequireRoleBasedAuthorization(Constants.AdminRole, Constants.UserRole);

        parcelGroup.MapPut(nameof(UpdateParcelStatus), UpdateParcelStatus)
            .Produces<UpdateParcelStatusResponse>()
            .Produces<ParcelNotFoundResponse>((int)HttpStatusCode.NotFound)
            .RequireRoleBasedAuthorization(Constants.AdminRole, Constants.CourierRole);

        parcelGroup.MapPut(nameof(AssignParcel), AssignParcel)
            .Produces<AssignParcelResponse>()
            .Produces<ParcelNotFoundResponse>((int)HttpStatusCode.NotFound)
            .RequireRoleBasedAuthorization(Constants.AdminRole);

        parcelGroup.MapDelete(nameof(CancelParcel), CancelParcel)
            .Produces<CancelParcelResponse>()
            .Produces<ParcelNotFoundResponse>((int)HttpStatusCode.NotFound)
            .RequireRoleBasedAuthorization(Constants.AdminRole, Constants.UserRole);
    }


    private static async Task<IResult> RegisterUser(
        [FromBody] UserRegistrationRequest request,
        [FromServices] IRequestClient<UserRegistrationRequest> client,
        CancellationToken token)
    {
        Response<AuthorizationResponse, RegistrationFailedResponse> response =
            await client.GetResponse<AuthorizationResponse, RegistrationFailedResponse>(request, token);

        return response.Is<RegistrationFailedResponse>(out Response<RegistrationFailedResponse> result)
            ? Results.Problem(new()
            {
                Status = (int?)HttpStatusCode.InternalServerError,
                Detail = result.Message.Message,
                Type = nameof(RegistrationFailedResponse),
                Title = "Failed to process registration request"
            })
            : Results.Ok(response.Message);
    }

    private static async Task<IResult> RegisterCourier(
        [FromBody] CourierRegistrationRequest request,
        [FromServices] IRequestClient<CourierRegistrationRequest> client,
        CancellationToken token)
    {
        Response<AuthorizationResponse, RegistrationFailedResponse> response =
            await client.GetResponse<AuthorizationResponse, RegistrationFailedResponse>(request, token);

        return response.Is<RegistrationFailedResponse>(out Response<RegistrationFailedResponse> result)
            ? Results.Problem(new()
            {
                Status = (int?)HttpStatusCode.InternalServerError,
                Detail = result.Message.Message,
                Type = nameof(RegistrationFailedResponse),
                Title = "Failed to process registration request"
            })
            : Results.Ok(response.Message);
    }

    private static async Task<IResult> Login(
        [FromBody] LoginRequest request,
        [FromServices] IRequestClient<LoginRequest> client,
        CancellationToken token)
    {
        Response<AuthorizationResponse, UnauthorizedResponse> response =
            await client.GetResponse<AuthorizationResponse, UnauthorizedResponse>(request, token);

        return response.Is<UnauthorizedResponse>(out Response<UnauthorizedResponse> _)
            ? Results.Unauthorized()
            : Results.Ok(response.Message);
    }

    private static async Task<IResult> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        [FromServices] IRequestClient<RefreshTokenRequest> client,
        CancellationToken token)
    {
        Response<AuthorizationResponse, UnauthorizedResponse> response =
            await client.GetResponse<AuthorizationResponse, UnauthorizedResponse>(request, token);

        return response.Is<UnauthorizedResponse>(out Response<UnauthorizedResponse> _)
            ? Results.Unauthorized()
            : Results.Ok(response.Message);
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
        [AsParameters] GetParcelRequest request,
        [FromServices] IRequestClient<GetParcelRequest> client,
        CancellationToken token)
    {
        Response<GetParcelResponse> response = await client.GetResponse<GetParcelResponse>(request, token);

        return response.Message;
    }

    /// <summary>
    /// Gets all parcels.
    /// </summary>
    /// <returns>The list of parcels.</returns>
    private static async Task<IResult> GetParcels(
        [FromServices] IRequestClient<GetParcelsRequest> client,
        CancellationToken token)
    {
        Response<GetParcelsResponse> response = await client.GetResponse<GetParcelsResponse>(new(), token);

        return Results.Ok(response.Message);
    }

    /// <summary>
    /// Gets parcels by user ID.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>The list of parcels for the user.</returns>
    private static async Task<IResult> GetParcelsByUser(
        [AsParameters] GetParcelsByUserRequest request,
        [FromServices] IRequestClient<GetParcelsByUserRequest> client,
        CancellationToken token)
    {
        Response<GetParcelsByUserResponse> response = await client.GetResponse<GetParcelsByUserResponse>(request, token);

        return Results.Ok(response.Message);
    }

    /// <summary>
    /// Gets parcels by courier ID.
    /// </summary>
    /// <param name="courierId">The ID of the courier.</param>
    /// <returns>The list of parcels for the courier.</returns>
    private static async Task<IResult> GetParcelsByCourier(
        [AsParameters] GetParcelsByCourierRequest request,
        [FromServices] IRequestClient<GetParcelsByCourierRequest> client,
        CancellationToken token)
    {
        Response<GetParcelsByCourierResponse> response = await client.GetResponse<GetParcelsByCourierResponse>(request, token);

        return Results.Ok(response.Message);
    }

    /// <summary>
    /// Updates the destination of a parcel.
    /// </summary>
    /// <param name="request">The request containing the new destination details.</param>
    /// <returns>The result of the update destination operation.</returns>
    private static async Task<IResult> UpdateDestination(
        [FromBody] UpdateDestinationRequest request,
        [FromServices] IRequestClient<UpdateDestinationRequest> client,
        CancellationToken token)
    {
        Response<UpdateDestinationResponse, ParcelNotFoundResponse> response =
            await client.GetResponse<UpdateDestinationResponse, ParcelNotFoundResponse>(request, token);

        return response.Is<ParcelNotFoundResponse>(out Response<ParcelNotFoundResponse> result)
            ? Results.NotFound(result.Message)
            : Results.Ok(response.Message);
    }

    /// <summary>
    /// Updates the status of a parcel.
    /// </summary>
    /// <param name="request">The request containing the new status details.</param>
    /// <returns>The result of the update parcel status operation.</returns>
    private static async Task<IResult> UpdateParcelStatus(
        [FromBody] UpdateParcelStatusRequest request,
        [FromServices] IRequestClient<UpdateParcelStatusRequest> client,
        CancellationToken token)
    {
        Response<UpdateParcelStatusResponse, ParcelNotFoundResponse> response =
            await client.GetResponse<UpdateParcelStatusResponse, ParcelNotFoundResponse>(request, token);

        return response.Is<ParcelNotFoundResponse>(out Response<ParcelNotFoundResponse> result)
            ? Results.NotFound(result.Message)
            : Results.Ok(response.Message);
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
    private static async Task<IResult> CancelParcel(
        [FromBody] CancelParcelRequest request,
        [FromServices] IRequestClient<CancelParcelRequest> client,
        CancellationToken token)
    {
        Response<CancelParcelResponse, ParcelNotFoundResponse> response =
            await client.GetResponse<CancelParcelResponse, ParcelNotFoundResponse>(request, token);

        return response.Is<ParcelNotFoundResponse>(out Response<ParcelNotFoundResponse> result)
            ? Results.NotFound(result.Message)
            : Results.Ok(response.Message);
    }


    private static void RequireRoleBasedAuthorization(this RouteHandlerBuilder builder, params string[] roles) =>
        builder.RequireAuthorization(policyBuilder => policyBuilder.RequireRole(roles));
}