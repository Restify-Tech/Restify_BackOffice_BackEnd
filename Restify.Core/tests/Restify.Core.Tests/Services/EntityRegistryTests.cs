using FluentAssertions;
using Restify.Core.Application.DTOs.Grid;
using Restify.Core.Domain.Entities;
using Restify.Core.Infrastructure.Persistence;
using Restify.Core.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Restify.Core.Tests.Services;

public class EntityRegistryTests
{
    private readonly EntityRegistry _registry;

    public EntityRegistryTests()
    {
        _registry = new EntityRegistry();
    }

    [Fact]
    public void Register_ConDatosValidos_RegistraEntidad()
    {
        // Act
        _registry.Register<GridConfiguration, CoreDbContext>("GridConfiguration");

        // Assert
        _registry.IsRegistered("GridConfiguration").Should().BeTrue();
    }

    [Fact]
    public void GetRegistration_ConEntidadRegistrada_RetornaRegistro()
    {
        // Arrange
        _registry.Register<GridConfiguration, CoreDbContext>("GridConfiguration");

        // Act
        var registration = _registry.GetRegistration("GridConfiguration");

        // Assert
        registration.Should().NotBeNull();
        registration!.EntityName.Should().Be("GridConfiguration");
        registration.EntityType.Should().Be(typeof(GridConfiguration));
    }

    [Fact]
    public void GetRegistration_ConEntidadNoRegistrada_RetornaNull()
    {
        // Act
        var registration = _registry.GetRegistration("NonExistent");

        // Assert
        registration.Should().BeNull();
    }

    [Fact]
    public void GetRegisteredEntities_RetornaTodasLasEntidades()
    {
        // Arrange
        _registry.Register<GridConfiguration, CoreDbContext>("GridConfiguration");
        _registry.Register<GeneralTable, CoreDbContext>("GeneralTable");
        _registry.Register<GeneralValue, CoreDbContext>("GeneralValue");

        // Act
        var entities = _registry.GetRegisteredEntities();

        // Assert
        entities.Should().HaveCount(3);
        entities.Should().Contain("GridConfiguration");
        entities.Should().Contain("GeneralTable");
        entities.Should().Contain("GeneralValue");
    }

    [Fact]
    public void IsRegistered_ConEntidadRegistrada_RetornaTrue()
    {
        // Arrange
        _registry.Register<GridConfiguration, CoreDbContext>("GridConfiguration");

        // Act & Assert
        _registry.IsRegistered("GridConfiguration").Should().BeTrue();
    }

    [Fact]
    public void IsRegistered_ConEntidadNoRegistrada_RetornaFalse()
    {
        // Act & Assert
        _registry.IsRegistered("NonExistent").Should().BeFalse();
    }

    [Fact]
    public void Register_MultiplesEntidades_SinConflicto()
    {
        // Act
        _registry.Register<GridConfiguration, CoreDbContext>("GridConfiguration");
        _registry.Register<GeneralTable, CoreDbContext>("GeneralTable");

        // Assert
        _registry.GetRegisteredEntities().Should().HaveCount(2);
    }
}
