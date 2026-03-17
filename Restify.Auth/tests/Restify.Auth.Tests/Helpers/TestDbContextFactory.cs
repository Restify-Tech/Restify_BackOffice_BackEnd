using Microsoft.EntityFrameworkCore;
using Moq;
using Restify.Auth.Application.Interfaces;
using Restify.Auth.Infrastructure.Persistence;

namespace Restify.Auth.Tests.Helpers;

/// <summary>
/// Factory para crear AppDbContext con InMemory database para tests
/// </summary>
public static class TestDbContextFactory
{
    /// <summary>
    /// Crea un AppDbContext sin filtro de tenant (para AuthService que usa IgnoreQueryFilters)
    /// </summary>
    public static AppDbContext Create(string? dbName = null)
    {
        dbName ??= Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new AppDbContext(options);
    }

    /// <summary>
    /// Crea un AppDbContext con filtro de tenant activo
    /// </summary>
    public static AppDbContext CreateWithTenant(Guid tenantId, string? dbName = null)
    {
        dbName ??= Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        var currentUserMock = new Mock<ICurrentUserService>();
        currentUserMock.Setup(x => x.TenantId).Returns(tenantId);
        currentUserMock.Setup(x => x.UserId).Returns(Guid.NewGuid());

        return new AppDbContext(options, currentUserMock.Object);
    }
}
