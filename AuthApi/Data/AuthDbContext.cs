using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using NetDevPack.Security.Jwt.Core.Model;


namespace AuthApi.Data;

public class AuthDbContext(DbContextOptions<AuthDbContext> options) : IdentityDbContext(options), IAuthDbContext
{
    public DbSet<KeyMaterial> SecurityKeys { get; set; }
}