using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using NetDevPack.Security.Jwt.Core.Model;
using NetDevPack.Security.Jwt.Store.EntityFrameworkCore;


namespace AuthApi.Data;

public class AuthDbContext(DbContextOptions<AuthDbContext> options)
    : IdentityDbContext<DomainUser, IdentityRole<int>, int>(options), IAuthDbContext, ISecurityKeyContext
{
    public DbSet<KeyMaterial> SecurityKeys { get; set; }
}