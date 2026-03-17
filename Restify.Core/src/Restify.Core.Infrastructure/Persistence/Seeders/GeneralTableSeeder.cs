using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Restify.Core.Domain.Entities;

namespace Restify.Core.Infrastructure.Persistence.Seeders;

/// <summary>
/// Seeder para GeneralTables y GeneralValues
/// </summary>
public static class GeneralTableSeeder
{
    private static readonly Guid SystemTenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<CoreDbContext>>();

        try
        {
            // Verificar si ya existe data
            var exists = await context.GeneralTables
                .IgnoreQueryFilters()
                .AnyAsync();

            if (exists)
            {
                logger.LogInformation("GeneralTables already seeded, skipping...");
                return;
            }

            logger.LogInformation("Seeding GeneralTables and GeneralValues...");

            var tables = GetInitialTables();
            await context.GeneralTables.AddRangeAsync(tables);
            await context.SaveChangesAsync();

            logger.LogInformation("GeneralTables seeded successfully. Created {Count} tables", tables.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding GeneralTables");
            throw;
        }
    }

    private static List<GeneralTable> GetInitialTables()
    {
        var tables = new List<GeneralTable>();

        // ESTADOS_PEDIDO - Estados de un pedido
        var estadosPedido = CreateTable("ESTADOS_PEDIDO", "Estados de Pedido", "Estados posibles de un pedido", "PEDIDOS", true);
        estadosPedido.Values = new List<GeneralValue>
        {
            CreateValue(estadosPedido.Id, "PENDIENTE", "Pendiente", "Pedido pendiente de preparación", 1, "#FFA500", "#FFFFFF", 0, true, true),
            CreateValue(estadosPedido.Id, "EN_PREPARACION", "En Preparación", "Pedido en cocina", 2, "#2196F3", "#FFFFFF", 1, true, false),
            CreateValue(estadosPedido.Id, "LISTO", "Listo", "Pedido listo para entregar", 3, "#4CAF50", "#FFFFFF", 2, true, false),
            CreateValue(estadosPedido.Id, "ENTREGADO", "Entregado", "Pedido entregado al cliente", 4, "#9E9E9E", "#FFFFFF", 3, true, false),
            CreateValue(estadosPedido.Id, "CANCELADO", "Cancelado", "Pedido cancelado", 5, "#F44336", "#FFFFFF", 4, true, false)
        };
        tables.Add(estadosPedido);

        // TIPOS_PAGO - Formas de pago
        var tiposPago = CreateTable("TIPOS_PAGO", "Tipos de Pago", "Métodos de pago aceptados", "FACTURACION", true);
        tiposPago.Values = new List<GeneralValue>
        {
            CreateValue(tiposPago.Id, "EFECTIVO", "Efectivo", "Pago en efectivo", 1, "#4CAF50", "#FFFFFF", 0, true, true),
            CreateValue(tiposPago.Id, "TARJETA_CREDITO", "Tarjeta de Crédito", "Pago con tarjeta de crédito", 2, "#2196F3", "#FFFFFF", 1, true, false),
            CreateValue(tiposPago.Id, "TARJETA_DEBITO", "Tarjeta de Débito", "Pago con tarjeta de débito", 3, "#03A9F4", "#FFFFFF", 2, true, false),
            CreateValue(tiposPago.Id, "TRANSFERENCIA", "Transferencia", "Transferencia bancaria", 4, "#9C27B0", "#FFFFFF", 3, true, false),
            CreateValue(tiposPago.Id, "BILLETERA_DIGITAL", "Billetera Digital", "Yape, Plin, etc.", 5, "#FF9800", "#FFFFFF", 4, false, false)
        };
        tables.Add(tiposPago);

        // ESTADOS_MESA - Estados de una mesa
        var estadosMesa = CreateTable("ESTADOS_MESA", "Estados de Mesa", "Estados posibles de una mesa", "MESAS", true);
        estadosMesa.Values = new List<GeneralValue>
        {
            CreateValue(estadosMesa.Id, "DISPONIBLE", "Disponible", "Mesa disponible", 1, "#4CAF50", "#FFFFFF", 0, true, true),
            CreateValue(estadosMesa.Id, "OCUPADA", "Ocupada", "Mesa ocupada", 2, "#F44336", "#FFFFFF", 1, true, false),
            CreateValue(estadosMesa.Id, "RESERVADA", "Reservada", "Mesa reservada", 3, "#FFA500", "#FFFFFF", 2, true, false),
            CreateValue(estadosMesa.Id, "EN_LIMPIEZA", "En Limpieza", "Mesa en proceso de limpieza", 4, "#2196F3", "#FFFFFF", 3, true, false),
            CreateValue(estadosMesa.Id, "FUERA_SERVICIO", "Fuera de Servicio", "Mesa no disponible", 5, "#9E9E9E", "#FFFFFF", 4, true, false)
        };
        tables.Add(estadosMesa);

        // TIPOS_PRODUCTO - Tipos de productos
        var tiposProducto = CreateTable("TIPOS_PRODUCTO", "Tipos de Producto", "Clasificación de productos", "MENU", true);
        tiposProducto.Values = new List<GeneralValue>
        {
            CreateValue(tiposProducto.Id, "COMIDA", "Comida", "Plato de comida", 1, "#FF5722", "#FFFFFF", 0, true, true),
            CreateValue(tiposProducto.Id, "BEBIDA", "Bebida", "Bebida", 2, "#2196F3", "#FFFFFF", 1, true, false),
            CreateValue(tiposProducto.Id, "POSTRE", "Postre", "Postre o dulce", 3, "#E91E63", "#FFFFFF", 2, true, false),
            CreateValue(tiposProducto.Id, "EXTRA", "Extra", "Complemento o adicional", 4, "#9C27B0", "#FFFFFF", 3, true, false),
            CreateValue(tiposProducto.Id, "COMBO", "Combo", "Combinación de productos", 5, "#4CAF50", "#FFFFFF", 4, true, false)
        };
        tables.Add(tiposProducto);

        // UNIDADES_MEDIDA - Unidades de medida
        var unidadesMedida = CreateTable("UNIDADES_MEDIDA", "Unidades de Medida", "Unidades para medir cantidades", "INVENTARIO", true);
        unidadesMedida.Values = new List<GeneralValue>
        {
            CreateValue(unidadesMedida.Id, "UND", "Unidad", "Unidad individual", 1, null, null, 0, true, true),
            CreateValue(unidadesMedida.Id, "KG", "Kilogramo", "Kilogramo (1000g)", 1000, null, null, 1, true, false),
            CreateValue(unidadesMedida.Id, "GR", "Gramo", "Gramo", 1, null, null, 2, true, false),
            CreateValue(unidadesMedida.Id, "LT", "Litro", "Litro (1000ml)", 1000, null, null, 3, true, false),
            CreateValue(unidadesMedida.Id, "ML", "Mililitro", "Mililitro", 1, null, null, 4, true, false),
            CreateValue(unidadesMedida.Id, "PZA", "Pieza", "Pieza individual", 1, null, null, 5, true, false),
            CreateValue(unidadesMedida.Id, "PORCION", "Porción", "Porción estándar", 1, null, null, 6, true, false)
        };
        tables.Add(unidadesMedida);

        // TIPOS_DOCUMENTO - Tipos de documento de identidad
        var tiposDocumento = CreateTable("TIPOS_DOCUMENTO", "Tipos de Documento", "Tipos de documento de identidad", "CLIENTES", true);
        tiposDocumento.Values = new List<GeneralValue>
        {
            CreateValue(tiposDocumento.Id, "DNI", "DNI", "Documento Nacional de Identidad", null, null, null, 0, true, true),
            CreateValue(tiposDocumento.Id, "RUC", "RUC", "Registro Único de Contribuyentes", null, null, null, 1, true, false),
            CreateValue(tiposDocumento.Id, "CE", "Carnet de Extranjería", "Carnet de Extranjería", null, null, null, 2, true, false),
            CreateValue(tiposDocumento.Id, "PASAPORTE", "Pasaporte", "Pasaporte", null, null, null, 3, true, false)
        };
        tables.Add(tiposDocumento);

        // TIPOS_COMPROBANTE - Tipos de comprobante
        var tiposComprobante = CreateTable("TIPOS_COMPROBANTE", "Tipos de Comprobante", "Tipos de comprobante fiscal", "FACTURACION", true);
        tiposComprobante.Values = new List<GeneralValue>
        {
            CreateValue(tiposComprobante.Id, "BOLETA", "Boleta de Venta", "Boleta de venta electrónica", null, null, null, 0, true, true),
            CreateValue(tiposComprobante.Id, "FACTURA", "Factura", "Factura electrónica", null, null, null, 1, true, false),
            CreateValue(tiposComprobante.Id, "NOTA_CREDITO", "Nota de Crédito", "Nota de crédito electrónica", null, null, null, 2, true, false),
            CreateValue(tiposComprobante.Id, "NOTA_DEBITO", "Nota de Débito", "Nota de débito electrónica", null, null, null, 3, true, false),
            CreateValue(tiposComprobante.Id, "TICKET", "Ticket", "Ticket de venta (no fiscal)", null, null, null, 4, false, false)
        };
        tables.Add(tiposComprobante);

        // PRIORIDADES - Prioridades generales
        var prioridades = CreateTable("PRIORIDADES", "Prioridades", "Niveles de prioridad", "SISTEMA", true);
        prioridades.Values = new List<GeneralValue>
        {
            CreateValue(prioridades.Id, "BAJA", "Baja", "Prioridad baja", 1, "#4CAF50", "#FFFFFF", 0, true, false),
            CreateValue(prioridades.Id, "MEDIA", "Media", "Prioridad media", 2, "#FFA500", "#FFFFFF", 1, true, true),
            CreateValue(prioridades.Id, "ALTA", "Alta", "Prioridad alta", 3, "#F44336", "#FFFFFF", 2, true, false),
            CreateValue(prioridades.Id, "URGENTE", "Urgente", "Prioridad urgente", 4, "#9C27B0", "#FFFFFF", 3, true, false)
        };
        tables.Add(prioridades);

        return tables;
    }

    private static GeneralTable CreateTable(string code, string name, string description, string applicationCode, bool isInternal)
    {
        return new GeneralTable
        {
            Id = Guid.NewGuid(),
            TenantId = SystemTenantId,
            Code = code,
            Name = name,
            Description = description,
            ApplicationCode = applicationCode,
            IsInternal = isInternal,
            IsActive = true,
            DisplayOrder = 0,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };
    }

    private static GeneralValue CreateValue(
        Guid tableId,
        string code,
        string content,
        string? shortDescription,
        decimal? numericValue,
        string? backgroundColor,
        string? textColor,
        int displayOrder,
        bool isLocked,
        bool isDefault)
    {
        return new GeneralValue
        {
            Id = Guid.NewGuid(),
            TenantId = SystemTenantId,
            GeneralTableId = tableId,
            Code = code,
            Content = content,
            ShortDescription = shortDescription,
            NumericValue = numericValue,
            BackgroundColor = backgroundColor,
            TextColor = textColor,
            DisplayOrder = displayOrder,
            IsLocked = isLocked,
            IsActive = true,
            IsDefault = isDefault,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };
    }
}
