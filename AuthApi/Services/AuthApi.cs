using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using AuthApi.Data;

using Core.Commands.Auth;
using Core.Common;
using Core.Common.Enums;
using Core.Common.ErrorResponses;
using Core.Common.Options;

using MassTransit;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

using NetDevPack.Security.Jwt.Core.Interfaces;

using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;


namespace AuthApi.Services;

public class AuthApi(
    IOptions<AuthOptions> options,
    UserManager<DomainUser> userManager,
    RoleManager<IdentityRole<int>> roleManager,
    IJwtService jwtService,
    ILogger<AuthApi> logger) :
    IConsumer<UserRegistrationRequest>,
    IConsumer<CourierRegistrationRequest>,
    IConsumer<LoginRequest>,
    IConsumer<RefreshTokenRequest>
{
    private readonly AuthOptions _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

    public async Task Consume(ConsumeContext<UserRegistrationRequest> context)
    {
        try
        {
            object result = await this.RegisterAsync(context.Message.Username,
                context.Message.Password,
                Constants.UserRole,
                UserType.User);

            await context.RespondAsync(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "failed to register {Username} due to: {ErrorMessage}",
                context.Message.Username,
                ex.Message);

            await context.RespondAsync(RegistrationFailedResponse.CommonError(ex));
        }
    }

    public async Task Consume(ConsumeContext<CourierRegistrationRequest> context)
    {
        try
        {
            object result = await this.RegisterAsync(
                context.Message.Username,
                context.Message.Password,
                Constants.CourierRole,
                context.Message.UserType);

            await context.RespondAsync(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "failed to register {Username} due to: {ErrorMessage}",
                context.Message.Username,
                ex.Message);

            await context.RespondAsync(RegistrationFailedResponse.CommonError(ex));
        }
    }

    public async Task Consume(ConsumeContext<LoginRequest> context)
    {
        try
        {
            DomainUser user = await userManager.FindByNameAsync(context.Message.Username);

            if (user is null)
            {
                logger.LogWarning("{Username} unauthorized attempt", context.Message.Username);

                await context.RespondAsync(
                    UnauthorizedResponse.Unauthorized(context.Message.Username, context.Message.Role));

                return;
            }

            logger.LogDebug("{Username} user found", user.UserName);

            bool passwordCorrect = await userManager.CheckPasswordAsync(user, context.Message.Password);

            if (!passwordCorrect)
            {
                logger.LogWarning("{Username} unauthorized attempt with invalid password", context.Message.Username);

                await context.RespondAsync(
                    UnauthorizedResponse.Unauthorized(context.Message.Username, context.Message.Role));

                return;
            }

            if (!await userManager.IsInRoleAsync(user, context.Message.Role))
            {
                logger.LogWarning("{Username} unauthorized attempt", context.Message.Username);

                await context.RespondAsync(
                    UnauthorizedResponse.Unauthorized(context.Message.Username, context.Message.Role));

                return;
            }

            logger.LogDebug("{Username} user is in role {Role}", user.UserName, context.Message.Role);

            await context.RespondAsync<AuthorizationResponse>(new()
            {
                UserId = user.Id,
                AccessToken = await this.GenerateAccessToken(user),
                RefreshToken = await this.GenerateRefreshToken(user)
            });

            logger.LogInformation("{Username} logged in with {Role} role",
                context.Message.Username,
                context.Message.Role);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "failed to log in {Username} due to {Error}", context.Message.Username, ex.Message);
        }
    }

    public async Task Consume(ConsumeContext<RefreshTokenRequest> context)
    {
        try
        {
            var handler = new JsonWebTokenHandler();

            logger.LogDebug("validating token");

            TokenValidationResult tokenValidationResult = await handler.ValidateTokenAsync(
                context.Message.RefreshToken,
                new()
                {
                    ValidIssuer = _options.JwtIssuer,
                    ValidAudience = _options.JwtAudience,
                    RequireSignedTokens = false,
                    IssuerSigningKey = await jwtService.GetCurrentSecurityKey(),
                });

            if (!tokenValidationResult.IsValid)
            {
                logger.LogWarning("token validation failed");

                await context.RespondAsync(UnauthorizedResponse.TokenNotValid(context.Message.RefreshToken));

                return;
            }

            DomainUser user = await userManager.FindByNameAsync(context.Message.Username);

            if (user is null)
            {
                logger.LogWarning("{Username} not found", context.Message.Username);

                await context.RespondAsync(
                    UnauthorizedResponse.Unauthorized(context.Message.Username, context.Message.Role));

                return;
            }

            logger.LogDebug("{Username} user found", user.UserName);

            if (!await userManager.IsInRoleAsync(user, context.Message.Role))
            {
                logger.LogWarning("{Username} is not in the {Role} role",
                    context.Message.Username,
                    context.Message.Role);

                await context.RespondAsync(
                    UnauthorizedResponse.Unauthorized(context.Message.Username, context.Message.Role));

                return;
            }

            await context.RespondAsync<AuthorizationResponse>(new()
            {
                UserId = user.Id,
                AccessToken = await this.GenerateAccessToken(user),
                RefreshToken = await this.GenerateRefreshToken(user)
            });

            logger.LogInformation("token validated for {Username}", context.Message.Username);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "failed to create refresh token for {Username} due to {Error}",
                context.Message.Username,
                ex.Message);

            await context.RespondAsync(
                UnauthorizedResponse.Unauthorized(context.Message.Username, context.Message.Role));
        }
    }


    private async Task<object> RegisterAsync(string username, string password, string role, UserType userType)
    {
        var user = new DomainUser
        {
            UserType = userType,
            UserName = username,
            Email = username,
            EmailConfirmed = true
        };

        logger.LogDebug("creating {Username} user", user.UserName);

        IdentityResult result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            logger.LogWarning("failed to register {Username}: {@Errors}", username, result.Errors);

            return RegistrationFailedResponse.IdentityError(result);
        }

        logger.LogDebug("{Username} user created", user.UserName);

        result = await this.AddToRole(user, role);

        if (!result.Succeeded)
        {
            logger.LogWarning("failed to register {Username}: {@Errors}", username, result.Errors);

            return RegistrationFailedResponse.IdentityError(result);
        }

        logger.LogDebug("{Username} added to role {Role}", user.UserName, role);

        logger.LogInformation("{Username} registered with {Role} role", username, role);

        return new AuthorizationResponse
        {
            UserId = user.Id,
            AccessToken = await this.GenerateAccessToken(user),
            RefreshToken = await this.GenerateRefreshToken(user)
        };
    }

    private async Task<IdentityResult> AddToRole(DomainUser user, string role)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            logger.LogDebug("creating role {Role}", role);

            await roleManager.CreateAsync(new IdentityRole<int>(role));
        }

        logger.LogDebug("adding user {Username} to role {Role}", user.UserName, role);

        return await userManager.AddToRoleAsync(user, role);
    }

    private async Task<string> GenerateAccessToken(DomainUser user)
    {
        IList<string> userRoles = await userManager.GetRolesAsync(user);
        var identityClaims = new ClaimsIdentity();
        identityClaims.AddClaims(await userManager.GetClaimsAsync(user));
        identityClaims.AddClaims(userRoles.Select(s => new Claim("role", s)));
        identityClaims.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
        identityClaims.AddClaim(new Claim(JwtRegisteredClaimNames.Email, user.Email!));
        identityClaims.AddClaim(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        identityClaims.AddClaim(new(JwtRegisteredClaimNames.Aud, _options.JwtAudience));

        var handler = new JwtSecurityTokenHandler();
        SigningCredentials key = await jwtService.GetCurrentSigningCredentials();

        SecurityToken securityToken = handler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = _options.JwtIssuer, // <- Your website
            Audience = _options.JwtAudience,
            SigningCredentials = key,
            Subject = identityClaims,
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.Add(_options.TokenLifetime),
            IssuedAt = DateTime.UtcNow,
            TokenType = "at+jwt"
        });

        string encodedJwt = handler.WriteToken(securityToken);

        return encodedJwt;
    }

    private async Task<string> GenerateRefreshToken(DomainUser user)
    {
        var jti = Guid.NewGuid().ToString();

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, jti)
        };

        ClaimsIdentity identityClaims = new(claims);

        var handler = new JwtSecurityTokenHandler();

        SecurityToken securityToken = handler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = _options.JwtIssuer, // <- Your website
            Audience = _options.JwtAudience,
            SigningCredentials = await jwtService.GetCurrentSigningCredentials(),
            Subject = identityClaims,
            NotBefore = DateTime.Now,
            Expires = DateTime.Now.Add(_options.TokenLifetime),
            TokenType = "rt+jwt"
        });

        await this.UpdateLastGeneratedClaim(user, jti);

        return handler.WriteToken(securityToken);
    }

    private async Task UpdateLastGeneratedClaim(DomainUser user, string jti)
    {
        IList<Claim> claims = await userManager.GetClaimsAsync(user);
        var newLastRtClaim = new Claim("LastRefreshToken", jti);

        Claim claimLastRt = claims.FirstOrDefault(f => f.Type == "LastRefreshToken");

        switch (claimLastRt)
        {
            case null: await userManager.AddClaimAsync(user, newLastRtClaim); break;

            default: await userManager.ReplaceClaimAsync(user, claimLastRt, newLastRtClaim); break;
        }
    }
}