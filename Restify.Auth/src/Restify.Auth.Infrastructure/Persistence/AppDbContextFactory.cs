using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Restify.Auth.Infrastructure.Persistence;

/// <summary>
/// Factory para EF Core design-time (migraciones)
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5433;Database=restify_dev;Username=restify;Password=RestifyLocal2024!");

        return new AppDbContext(optionsBuilder.Options);
    }
}
