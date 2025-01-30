using Core.Common.Enums;

using Microsoft.AspNetCore.Identity;


namespace AuthApi.Data;

public class DomainUser : IdentityUser<int>
{
    public UserType UserType { get; set; }
}