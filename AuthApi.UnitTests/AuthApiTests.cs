using AutoFixture.Xunit2;

using Core.Commands.Auth;

using FluentAssertions;

using MassTransit;

using Test.Utility;


namespace AuthApi.UnitTests;

public class AuthApiTests
{
    [Theory, InlineAutoMoqData]
    public async Task UserRegistration_CommonError_ShouldNotFail(
        ConsumeContext<UserRegistrationRequest> context,
        Services.AuthApi sut)
    {
        // Act & Assert
        await sut.Invoking(x => x.Consume(context)).Should().NotThrowAsync();
    }


    [Theory, InlineAutoMoqData]
    public async Task CourierRegistration_CommonError_ShouldNotFail(
        ConsumeContext<CourierRegistrationRequest> context,
        Services.AuthApi sut)
    {
        // Act & Assert
        await sut.Invoking(x => x.Consume(context)).Should().NotThrowAsync();
    }


    [Theory, InlineAutoMoqData]
    public async Task Login_CommonError_ShouldNotFail(ConsumeContext<LoginRequest> context, Services.AuthApi sut)
    {
        // Act & Assert
        await sut.Invoking(x => x.Consume(context)).Should().NotThrowAsync();
    }


    [Theory, InlineAutoMoqData]
    public async Task RefreshToken_CommonError_ShouldNotFail(
        ConsumeContext<RefreshTokenRequest> context,
        Services.AuthApi sut)
    {
        // Act & Assert
        await sut.Invoking(x => x.Consume(context)).Should().NotThrowAsync();
    }
}