using AuthApi.Data;

using AutoFixture.Xunit2;

using Core.Commands.Auth;
using Core.Common.Enums;
using Core.Common.ErrorResponses;

using FluentAssertions;

using MassTransit;

using Microsoft.AspNetCore.Identity;

using Test.Utility;


namespace AuthApi.IntegrationTests;

[Collection(nameof(IntegrationTestsCollection))]
public class AuthApiConsumerTests : IDisposable
{
    private readonly ParcelDeliveryWebApplicationFactory<Program, IAuthDbContext, AuthDbContext> _factory;

    public AuthApiConsumerTests()
    {
        _factory = new();
        _factory.EnsureDbCreated();
        _factory.TestHarness.Start();
    }

    public void Dispose()
    {
        _factory.EnsureDbDeleted();
        _factory.Dispose();
    }


    [Theory, AutoData]
    public async Task UserRegistrationRequest_NewUser_ShouldReturnAuthorizationResponse(UserRegistrationRequest request)
    {
        // Arrange
        IRequestClient<UserRegistrationRequest> client =
            _factory.TestHarness.GetRequestClient<UserRegistrationRequest>();

        // Act
        Response<AuthorizationResponse> response = await client.GetResponse<AuthorizationResponse>(request);

        // Assert
        await _factory.AssertResponse<Services.AuthApi, UserRegistrationRequest, AuthorizationResponse>();

        response.Message.UserId.Should().BePositive();
        response.Message.AccessToken.Should().NotBeNullOrEmpty();
        response.Message.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Theory, AutoData]
    public async Task UserRegistrationRequest_UserAlreadyExist_ShouldReturnRegistrationFailedResponse(
        UserRegistrationRequest request)
    {
        // Arrange
        DomainUser user = new()
        {
            UserName = request.Username,
            NormalizedUserName = request.Username.ToUpper(),
            Email = request.Username,
            NormalizedEmail = request.Username.ToUpper()
        };

        await _factory.SeedDataAsync([user]);

        IRequestClient<UserRegistrationRequest> client =
            _factory.TestHarness.GetRequestClient<UserRegistrationRequest>();

        // Act
        Response<AuthorizationResponse, RegistrationFailedResponse> response =
            await client.GetResponse<AuthorizationResponse, RegistrationFailedResponse>(request);

        // Assert
        await _factory.AssertResponse<Services.AuthApi, UserRegistrationRequest, RegistrationFailedResponse>();

        response.Message.Should().BeOfType<RegistrationFailedResponse>();
    }


    [Theory, AutoData]
    public async Task CourierRegistrationRequest_NewCourier_ShouldReturnAuthorizationResponse(
        CourierRegistrationRequest request)
    {
        // Arrange
        IRequestClient<CourierRegistrationRequest> client =
            _factory.TestHarness.GetRequestClient<CourierRegistrationRequest>();

        // Act
        Response<AuthorizationResponse> response = await client.GetResponse<AuthorizationResponse>(request);

        // Assert
        await _factory.AssertResponse<Services.AuthApi, CourierRegistrationRequest, AuthorizationResponse>();

        response.Message.UserId.Should().BePositive();
        response.Message.AccessToken.Should().NotBeNullOrEmpty();
        response.Message.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Theory, AutoData]
    public async Task CourierRegistrationRequest_CourierAlreadyExist_ShouldReturnRegistrationFailedResponse(
        CourierRegistrationRequest request)
    {
        // Arrange
        DomainUser user = new()
        {
            UserName = request.Username,
            NormalizedUserName = request.Username.ToUpper(),
            Email = request.Username,
            NormalizedEmail = request.Username.ToUpper()
        };

        await _factory.SeedDataAsync([user]);

        IRequestClient<CourierRegistrationRequest> client =
            _factory.TestHarness.GetRequestClient<CourierRegistrationRequest>();

        // Act
        Response<AuthorizationResponse, RegistrationFailedResponse> response =
            await client.GetResponse<AuthorizationResponse, RegistrationFailedResponse>(request);

        // Assert
        await _factory.AssertResponse<Services.AuthApi, CourierRegistrationRequest, RegistrationFailedResponse>();

        response.Message.Should().BeOfType<RegistrationFailedResponse>();
    }


    [Theory, AutoData]
    public async Task LoginRequest_UserExistPasswordAndRoleCorrect_ShouldReturnAuthorizationResponse(
        LoginRequest request)
    {
        // Arrange
        UserManager<DomainUser> userManager = _factory.GetRequiredService<UserManager<DomainUser>>();

        DomainUser user = new()
        {
            UserName = request.Username,
            NormalizedUserName = request.Username.ToUpper(),
            Email = request.Username,
            NormalizedEmail = request.Username.ToUpper()
        };

        user.PasswordHash = userManager.PasswordHasher.HashPassword(user, request.Password);

        IdentityRole<int> role = new()
        {
            Id = 1,
            Name = request.Role,
            NormalizedName = request.Role.ToUpper()
        };

        RoleManager<IdentityRole<int>> roleManager = _factory.GetRequiredService<RoleManager<IdentityRole<int>>>();

        await roleManager.CreateAsync(role);

        await userManager.CreateAsync(user);

        await userManager.AddToRoleAsync(user, role.Name);

        IRequestClient<LoginRequest> client = _factory.TestHarness.GetRequestClient<LoginRequest>();

        // Act
        Response<AuthorizationResponse> response = await client.GetResponse<AuthorizationResponse>(request);

        // Assert
        await _factory.AssertResponse<Services.AuthApi, LoginRequest, AuthorizationResponse>();

        response.Message.UserId.Should().Be(user.Id);
        response.Message.AccessToken.Should().NotBeNullOrEmpty();
        response.Message.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Theory, AutoData]
    public async Task LoginRequest_UserExistPasswordNotCorrect_ShouldReturnUnauthorizedResponse(LoginRequest request)
    {
        // Arrange
        UserManager<DomainUser> userManager = _factory.GetRequiredService<UserManager<DomainUser>>();

        DomainUser user = new()
        {
            UserName = request.Username,
            NormalizedUserName = request.Username.ToUpper(),
            Email = request.Username,
            NormalizedEmail = request.Username.ToUpper()
        };

        user.PasswordHash = userManager.PasswordHasher.HashPassword(user, "r@nDomP@55w0&d");

        await _factory.SeedDataAsync([user]);


        IRequestClient<LoginRequest> client = _factory.TestHarness.GetRequestClient<LoginRequest>();

        // Act
        Response<AuthorizationResponse, UnauthorizedResponse> response =
            await client.GetResponse<AuthorizationResponse, UnauthorizedResponse>(request);

        // Assert
        await _factory.AssertResponse<Services.AuthApi, LoginRequest, UnauthorizedResponse>();

        response.Message.Should().BeOfType<UnauthorizedResponse>();
    }

    [Theory, AutoData]
    public async Task LoginRequest_UserExistPasswordCorrectRoleNotCorrect_ShouldReturnUnauthorizedResponse(
        LoginRequest request)
    {
        // Arrange
        UserManager<DomainUser> userManager = _factory.GetRequiredService<UserManager<DomainUser>>();

        DomainUser user = new()
        {
            UserName = request.Username,
            NormalizedUserName = request.Username.ToUpper(),
            Email = request.Username,
            NormalizedEmail = request.Username.ToUpper()
        };

        user.PasswordHash = userManager.PasswordHasher.HashPassword(user, request.Password);

        await _factory.SeedDataAsync([user]);

        IRequestClient<LoginRequest> client = _factory.TestHarness.GetRequestClient<LoginRequest>();

        // Act
        Response<AuthorizationResponse, UnauthorizedResponse> response =
            await client.GetResponse<AuthorizationResponse, UnauthorizedResponse>(request);

        // Assert
        await _factory.AssertResponse<Services.AuthApi, LoginRequest, UnauthorizedResponse>();

        response.Message.Should().BeOfType<UnauthorizedResponse>();
    }


    [Theory, AutoData]
    public async Task RefreshTokenRequest_TokenCorrectUserExistInCorrectRole_ShouldReturnAuthorizationResponse(
        LoginRequest loginRequest)
    {
        // Arrange
        UserManager<DomainUser> userManager = _factory.GetRequiredService<UserManager<DomainUser>>();

        DomainUser user = new()
        {
            UserName = loginRequest.Username,
            NormalizedUserName = loginRequest.Username.ToUpper(),
            Email = loginRequest.Username,
            NormalizedEmail = loginRequest.Username.ToUpper()
        };

        user.PasswordHash = userManager.PasswordHasher.HashPassword(user, loginRequest.Password);

        IdentityRole<int> role = new()
        {
            Id = 1,
            Name = loginRequest.Role,
            NormalizedName = loginRequest.Role.ToUpper()
        };

        RoleManager<IdentityRole<int>> roleManager = _factory.GetRequiredService<RoleManager<IdentityRole<int>>>();

        await roleManager.CreateAsync(role);

        await userManager.CreateAsync(user);

        await userManager.AddToRoleAsync(user, role.Name);

        IRequestClient<LoginRequest> loginClient = _factory.TestHarness.GetRequestClient<LoginRequest>();

        Response<AuthorizationResponse> loginResponse =
            await loginClient.GetResponse<AuthorizationResponse>(loginRequest);

        RefreshTokenRequest refreshTokenRequest = new()
        {
            Username = loginRequest.Username,
            Role = loginRequest.Role,
            RefreshToken = loginResponse.Message.RefreshToken
        };

        IRequestClient<RefreshTokenRequest> refreshTokenClient =
            _factory.TestHarness.GetRequestClient<RefreshTokenRequest>();

        // Act
        Response<AuthorizationResponse> response =
            await refreshTokenClient.GetResponse<AuthorizationResponse>(refreshTokenRequest);

        // Assert
        await _factory.AssertResponse<Services.AuthApi, RefreshTokenRequest, AuthorizationResponse>();

        response.Message.UserId.Should().Be(user.Id);
        response.Message.AccessToken.Should().NotBeNullOrEmpty();
        response.Message.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Theory, AutoData]
    public async Task RefreshTokenRequest_TokenCorrectUserNameNotCorrect_ShouldReturnUnauthorizedResponse(
        LoginRequest loginRequest)
    {
        // Arrange
        UserManager<DomainUser> userManager = _factory.GetRequiredService<UserManager<DomainUser>>();

        DomainUser user = new()
        {
            UserName = loginRequest.Username,
            NormalizedUserName = loginRequest.Username.ToUpper(),
            Email = loginRequest.Username,
            NormalizedEmail = loginRequest.Username.ToUpper()
        };

        user.PasswordHash = userManager.PasswordHasher.HashPassword(user, loginRequest.Password);

        IdentityRole<int> role = new()
        {
            Id = 1,
            Name = loginRequest.Role,
            NormalizedName = loginRequest.Role.ToUpper()
        };

        RoleManager<IdentityRole<int>> roleManager = _factory.GetRequiredService<RoleManager<IdentityRole<int>>>();

        await roleManager.CreateAsync(role);

        await userManager.CreateAsync(user);

        await userManager.AddToRoleAsync(user, role.Name);

        IRequestClient<LoginRequest> loginClient = _factory.TestHarness.GetRequestClient<LoginRequest>();

        Response<AuthorizationResponse> loginResponse =
            await loginClient.GetResponse<AuthorizationResponse>(loginRequest);

        RefreshTokenRequest refreshTokenRequest = new()
        {
            Username = nameof(RefreshTokenRequest.Username),
            Role = loginRequest.Role,
            RefreshToken = loginResponse.Message.RefreshToken
        };

        IRequestClient<RefreshTokenRequest> refreshTokenClient =
            _factory.TestHarness.GetRequestClient<RefreshTokenRequest>();

        // Act
        Response<AuthorizationResponse, UnauthorizedResponse> response =
            await refreshTokenClient.GetResponse<AuthorizationResponse, UnauthorizedResponse>(refreshTokenRequest);

        // Assert
        await _factory.AssertResponse<Services.AuthApi, RefreshTokenRequest, UnauthorizedResponse>();

        response.Message.Should().BeOfType<UnauthorizedResponse>();
    }

    [Theory, AutoData]
    public async Task RefreshTokenRequest_TokenCorrectUserExistInDifferentRole_ShouldReturnUnauthorizedResponse(
        LoginRequest loginRequest)
    {
        // Arrange
        UserManager<DomainUser> userManager = _factory.GetRequiredService<UserManager<DomainUser>>();

        DomainUser user = new()
        {
            UserName = loginRequest.Username,
            NormalizedUserName = loginRequest.Username.ToUpper(),
            Email = loginRequest.Username,
            NormalizedEmail = loginRequest.Username.ToUpper()
        };

        user.PasswordHash = userManager.PasswordHasher.HashPassword(user, loginRequest.Password);

        IdentityRole<int> role = new()
        {
            Id = 1,
            Name = loginRequest.Role,
            NormalizedName = loginRequest.Role.ToUpper()
        };

        RoleManager<IdentityRole<int>> roleManager = _factory.GetRequiredService<RoleManager<IdentityRole<int>>>();

        await roleManager.CreateAsync(role);

        await userManager.CreateAsync(user);

        await userManager.AddToRoleAsync(user, role.Name);

        IRequestClient<LoginRequest> loginClient = _factory.TestHarness.GetRequestClient<LoginRequest>();

        Response<AuthorizationResponse> loginResponse =
            await loginClient.GetResponse<AuthorizationResponse>(loginRequest);

        RefreshTokenRequest refreshTokenRequest = new()
        {
            Username = loginRequest.Username,
            Role = nameof(RefreshTokenRequest.Role),
            RefreshToken = loginResponse.Message.RefreshToken
        };

        IRequestClient<RefreshTokenRequest> refreshTokenClient =
            _factory.TestHarness.GetRequestClient<RefreshTokenRequest>();

        // Act
        Response<AuthorizationResponse, UnauthorizedResponse> response =
            await refreshTokenClient.GetResponse<AuthorizationResponse, UnauthorizedResponse>(refreshTokenRequest);

        // Assert
        await _factory.AssertResponse<Services.AuthApi, RefreshTokenRequest, UnauthorizedResponse>();

        response.Message.Should().BeOfType<UnauthorizedResponse>();
    }

    [Theory, AutoData]
    public async Task RefreshTokenRequest_TokenNotCorrect_ShouldReturnUnauthorizedResponse(RefreshTokenRequest request)
    {
        // Arrange
        IRequestClient<RefreshTokenRequest> client = _factory.TestHarness.GetRequestClient<RefreshTokenRequest>();

        // Act
        Response<AuthorizationResponse, UnauthorizedResponse> response =
            await client.GetResponse<AuthorizationResponse, UnauthorizedResponse>(request);

        // Assert
        await _factory.AssertResponse<Services.AuthApi, RefreshTokenRequest, UnauthorizedResponse>();

        response.Message.Should().BeOfType<UnauthorizedResponse>();
    }
}