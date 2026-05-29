using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Restify.Auth.Domain.Constants;
using Restify.Auth.Domain.Entities;
using Restify.Auth.Domain.Enums;
using Restify.Auth.Infrastructure.Services;

namespace Restify.Auth.Infrastructure.Persistence;

/// <summary>
/// Servicio para sembrar datos iniciales en la base de datos
/// </summary>
public class DbSeeder(
    AppDbContext context,
    PasswordService passwordService,
    ILogger<DbSeeder> logger)
{
    private readonly AppDbContext _context = context;
    private readonly PasswordService _passwordService = passwordService;
    private readonly ILogger<DbSeeder> _logger = logger;

    /// <summary>
    /// Ejecuta la siembra de datos
    /// </summary>
    public async Task SeedAsync()
    {
        try
        {
            // Aplicar migraciones pendientes
            await _context.Database.MigrateAsync();

            // Sembrar permisos
            await SeedPermissionsAsync();

            // Agregar permisos nuevos (incremental)
            await SeedNewPermissionsAsync();

            // Sembrar pantallas-permisos
            await SeedScreenPermissionsAsync();

            // Sembrar planes de suscripción
            await SeedPlansAsync();

            // Sembrar tenant de demostración
            await SeedDemoTenantAsync();

            // Sembrar SuperAdmin
            await SeedSuperAdminAsync();

            // Sembrar GeneralValues (configuracion global)
            await SeedGeneralValuesAsync();

            await _context.SaveChangesAsync();

            _logger.LogInformation("Base de datos sembrada correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al sembrar la base de datos");
            throw;
        }
    }

    private async Task SeedPermissionsAsync()
    {
        if (await _context.Permissions.AnyAsync())
            return;

        var permissions = new List<Permission>
        {
            // Auth
            new() { Code = PermissionCodes.Auth.Manage, Name = "Gestionar autenticación", Module = "Auth", DisplayOrder = 1 },

            // Users
            new() { Code = PermissionCodes.Users.View, Name = "Ver usuarios", Module = "Users", DisplayOrder = 10, IsTenantDefault = true },
            new() { Code = PermissionCodes.Users.Create, Name = "Crear usuarios", Module = "Users", DisplayOrder = 11, IsTenantDefault = true },
            new() { Code = PermissionCodes.Users.Edit, Name = "Editar usuarios", Module = "Users", DisplayOrder = 12, IsTenantDefault = true },
            new() { Code = PermissionCodes.Users.Delete, Name = "Eliminar usuarios", Module = "Users", DisplayOrder = 13, IsTenantDefault = true },

            // Roles
            new() { Code = PermissionCodes.Roles.View, Name = "Ver roles", Module = "Roles", DisplayOrder = 20, IsTenantDefault = true },
            new() { Code = PermissionCodes.Roles.Create, Name = "Crear roles", Module = "Roles", DisplayOrder = 21, IsTenantDefault = true },
            new() { Code = PermissionCodes.Roles.Edit, Name = "Editar roles", Module = "Roles", DisplayOrder = 22, IsTenantDefault = true },
            new() { Code = PermissionCodes.Roles.Delete, Name = "Eliminar roles", Module = "Roles", DisplayOrder = 23, IsTenantDefault = true },

            // Tenant
            new() { Code = PermissionCodes.Tenant.View, Name = "Ver configuración", Module = "Tenant", DisplayOrder = 30, IsTenantDefault = true },
            new() { Code = PermissionCodes.Tenant.Edit, Name = "Editar configuración", Module = "Tenant", DisplayOrder = 31, IsTenantDefault = true },

            // Catalog
            new() { Code = PermissionCodes.Catalog.View, Name = "Ver catálogo", Module = "Catalog", DisplayOrder = 40, IsTenantDefault = true },
            new() { Code = PermissionCodes.Catalog.Create, Name = "Crear productos", Module = "Catalog", DisplayOrder = 41, IsTenantDefault = true },
            new() { Code = PermissionCodes.Catalog.Edit, Name = "Editar productos", Module = "Catalog", DisplayOrder = 42, IsTenantDefault = true },
            new() { Code = PermissionCodes.Catalog.Delete, Name = "Eliminar productos", Module = "Catalog", DisplayOrder = 43, IsTenantDefault = true },

            // Orders
            new() { Code = PermissionCodes.Orders.View, Name = "Ver pedidos", Module = "Orders", DisplayOrder = 50, IsTenantDefault = true },
            new() { Code = PermissionCodes.Orders.Create, Name = "Crear pedidos", Module = "Orders", DisplayOrder = 51, IsTenantDefault = true },
            new() { Code = PermissionCodes.Orders.Edit, Name = "Editar pedidos", Module = "Orders", DisplayOrder = 52, IsTenantDefault = true },
            new() { Code = PermissionCodes.Orders.Approve, Name = "Aprobar pedidos", Module = "Orders", DisplayOrder = 53, IsTenantDefault = true },
            new() { Code = PermissionCodes.Orders.Cancel, Name = "Cancelar pedidos", Module = "Orders", DisplayOrder = 54, IsTenantDefault = true },

            // Kitchen
            new() { Code = PermissionCodes.Kitchen.View, Name = "Ver cocina", Module = "Kitchen", DisplayOrder = 60, IsTenantDefault = true },
            new() { Code = PermissionCodes.Kitchen.Manage, Name = "Gestionar cocina", Module = "Kitchen", DisplayOrder = 61, IsTenantDefault = true },

            // Billing
            new() { Code = PermissionCodes.Billing.View, Name = "Ver facturación", Module = "Billing", DisplayOrder = 70, IsTenantDefault = true },
            new() { Code = PermissionCodes.Billing.Create, Name = "Crear facturas", Module = "Billing", DisplayOrder = 71, IsTenantDefault = true },
            new() { Code = PermissionCodes.Billing.Void, Name = "Anular facturas", Module = "Billing", DisplayOrder = 72, IsTenantDefault = true },
            new() { Code = PermissionCodes.Billing.CashierManage, Name = "Gestionar caja", Module = "Billing", DisplayOrder = 73, IsTenantDefault = true },

            // Reports
            new() { Code = PermissionCodes.Reports.View, Name = "Ver reportes", Module = "Reports", DisplayOrder = 80, IsTenantDefault = true },
            new() { Code = PermissionCodes.Reports.Export, Name = "Exportar reportes", Module = "Reports", DisplayOrder = 81, IsTenantDefault = true },

            // Delivery
            new() { Code = PermissionCodes.Delivery.View, Name = "Ver entregas", Module = "Delivery", DisplayOrder = 90, IsTenantDefault = true },
            new() { Code = PermissionCodes.Delivery.Manage, Name = "Gestionar entregas", Module = "Delivery", DisplayOrder = 91, IsTenantDefault = true },
            new() { Code = PermissionCodes.Delivery.Assign, Name = "Asignar entregas", Module = "Delivery", DisplayOrder = 92, IsTenantDefault = true },

            // Marketing
            new() { Code = PermissionCodes.Marketing.View, Name = "Ver marketing", Module = "Marketing", DisplayOrder = 100, IsTenantDefault = true },
            new() { Code = PermissionCodes.Marketing.Create, Name = "Crear campañas", Module = "Marketing", DisplayOrder = 101, IsTenantDefault = true },
            new() { Code = PermissionCodes.Marketing.Edit, Name = "Editar campañas", Module = "Marketing", DisplayOrder = 102, IsTenantDefault = true },
            new() { Code = PermissionCodes.Marketing.Delete, Name = "Eliminar campañas", Module = "Marketing", DisplayOrder = 103, IsTenantDefault = true },

            // Accounting
            new() { Code = PermissionCodes.Accounting.View, Name = "Ver contabilidad", Module = "Accounting", DisplayOrder = 110, IsTenantDefault = true },
            new() { Code = PermissionCodes.Accounting.Create, Name = "Crear asientos", Module = "Accounting", DisplayOrder = 111, IsTenantDefault = true },
            new() { Code = PermissionCodes.Accounting.Reports, Name = "Reportes contables", Module = "Accounting", DisplayOrder = 112, IsTenantDefault = true },

            // Management
            new() { Code = PermissionCodes.Management.View, Name = "Ver gestión", Module = "Management", DisplayOrder = 120, IsTenantDefault = true },
            new() { Code = PermissionCodes.Management.Approve, Name = "Aprobar operaciones", Module = "Management", DisplayOrder = 121, IsTenantDefault = true },
            new() { Code = PermissionCodes.Management.Settings, Name = "Configuración gerencial", Module = "Management", DisplayOrder = 122, IsTenantDefault = true },

            // Sales
            new() { Code = PermissionCodes.Sales.View, Name = "Ver ventas", Module = "Sales", DisplayOrder = 130, IsTenantDefault = true },
            new() { Code = PermissionCodes.Sales.Create, Name = "Crear ventas", Module = "Sales", DisplayOrder = 131, IsTenantDefault = true },
            new() { Code = PermissionCodes.Sales.Reports, Name = "Reportes de ventas", Module = "Sales", DisplayOrder = 132, IsTenantDefault = true },

            // Payroll
            new() { Code = PermissionCodes.Payroll.View, Name = "Ver nómina", Module = "Payroll", DisplayOrder = 140, IsTenantDefault = true },
            new() { Code = PermissionCodes.Payroll.Create, Name = "Crear nómina", Module = "Payroll", DisplayOrder = 141, IsTenantDefault = true },
            new() { Code = PermissionCodes.Payroll.Edit, Name = "Editar nómina", Module = "Payroll", DisplayOrder = 142, IsTenantDefault = true },
            new() { Code = PermissionCodes.Payroll.Reports, Name = "Reportes de nómina", Module = "Payroll", DisplayOrder = 143, IsTenantDefault = true },

            // Dispatch
            new() { Code = PermissionCodes.Dispatch.View, Name = "Ver despacho", Module = "Dispatch", DisplayOrder = 150, IsTenantDefault = true },
            new() { Code = PermissionCodes.Dispatch.Approve, Name = "Aprobar despacho", Module = "Dispatch", DisplayOrder = 151, IsTenantDefault = true },
            new() { Code = PermissionCodes.Dispatch.Manage, Name = "Gestionar despacho", Module = "Dispatch", DisplayOrder = 152, IsTenantDefault = true },

            // Customers
            new() { Code = PermissionCodes.Customers.View, Name = "Ver clientes", Module = "Customers", DisplayOrder = 160, IsTenantDefault = true },
            new() { Code = PermissionCodes.Customers.Create, Name = "Crear clientes", Module = "Customers", DisplayOrder = 161, IsTenantDefault = true },
            new() { Code = PermissionCodes.Customers.Edit, Name = "Editar clientes", Module = "Customers", DisplayOrder = 162, IsTenantDefault = true },
            new() { Code = PermissionCodes.Customers.Delete, Name = "Eliminar clientes", Module = "Customers", DisplayOrder = 163, IsTenantDefault = true },

            // AI
            new() { Code = PermissionCodes.AI.Generate, Name = "Generar con IA", Module = "AI", DisplayOrder = 170, IsTenantDefault = true },
            new() { Code = PermissionCodes.AI.Manage, Name = "Gestionar plantillas IA", Module = "AI", DisplayOrder = 171, IsTenantDefault = true },

            // SuperAdmin - Delivery Zones (NOT tenant default)
            new() { Code = PermissionCodes.DeliveryZones.View, Name = "Ver zonas de delivery", Module = "DeliveryZones", DisplayOrder = 200 },
            new() { Code = PermissionCodes.DeliveryZones.Create, Name = "Crear zonas de delivery", Module = "DeliveryZones", DisplayOrder = 201 },
            new() { Code = PermissionCodes.DeliveryZones.Edit, Name = "Editar zonas de delivery", Module = "DeliveryZones", DisplayOrder = 202 },
            new() { Code = PermissionCodes.DeliveryZones.Delete, Name = "Eliminar zonas de delivery", Module = "DeliveryZones", DisplayOrder = 203 },

            // SuperAdmin - Tenants (NOT tenant default)
            new() { Code = PermissionCodes.Tenants.View, Name = "Ver tenants", Module = "Tenants", DisplayOrder = 210 },
            new() { Code = PermissionCodes.Tenants.Edit, Name = "Editar tenants", Module = "Tenants", DisplayOrder = 211 },
            new() { Code = PermissionCodes.Tenants.Manage, Name = "Gestionar tenants", Module = "Tenants", DisplayOrder = 212 },

            // SuperAdmin - Pool Drivers (NOT tenant default)
            new() { Code = PermissionCodes.PoolDrivers.View, Name = "Ver repartidores pool", Module = "PoolDrivers", DisplayOrder = 220 },
            new() { Code = PermissionCodes.PoolDrivers.Manage, Name = "Gestionar repartidores pool", Module = "PoolDrivers", DisplayOrder = 221 },
            new() { Code = PermissionCodes.PoolDrivers.Verify, Name = "Verificar repartidores pool", Module = "PoolDrivers", DisplayOrder = 222 },

            // SuperAdmin - Commissions (NOT tenant default)
            new() { Code = PermissionCodes.Commissions.View, Name = "Ver comisiones", Module = "Commissions", DisplayOrder = 230 },
            new() { Code = PermissionCodes.Commissions.Manage, Name = "Gestionar comisiones", Module = "Commissions", DisplayOrder = 231 },
        };

        await _context.Permissions.AddRangeAsync(permissions);
        _logger.LogInformation("Permisos creados: {Count}", permissions.Count);
    }

    /// <summary>
    /// Agrega permisos nuevos que no existan en la BD (incremental, para actualizaciones)
    /// </summary>
    private async Task SeedNewPermissionsAsync()
    {
        var newPermissions = new List<(string Code, string Name, string Module, int DisplayOrder, bool IsTenantDefault)>
        {
            // Electronic Invoicing (Fase 9)
            (PermissionCodes.ElectronicInvoicing.View, "Ver facturación electrónica", "ElectronicInvoicing", 180, true),
            (PermissionCodes.ElectronicInvoicing.Emit, "Emitir comprobantes electrónicos", "ElectronicInvoicing", 181, true),
            (PermissionCodes.ElectronicInvoicing.Void, "Anular comprobantes electrónicos", "ElectronicInvoicing", 182, true),
            (PermissionCodes.ElectronicInvoicing.Configure, "Configurar facturación electrónica", "ElectronicInvoicing", 183, true),
        };

        var existingCodes = await _context.Permissions
            .Select(p => p.Code)
            .ToListAsync();

        var toAdd = newPermissions
            .Where(p => !existingCodes.Contains(p.Code))
            .Select(p => new Permission
            {
                Code = p.Code,
                Name = p.Name,
                Module = p.Module,
                DisplayOrder = p.DisplayOrder,
                IsTenantDefault = p.IsTenantDefault
            })
            .ToList();

        if (toAdd.Count > 0)
        {
            await _context.Permissions.AddRangeAsync(toAdd);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Permisos nuevos agregados: {Count}", toAdd.Count);

            // Asignar nuevos permisos a roles admin existentes (nombre "Administrador")
            var adminRoles = await _context.Roles
                .IgnoreQueryFilters()
                .Where(r => r.Name == SystemRoles.Administrador || r.Name == SystemRoles.SuperAdmin)
                .ToListAsync();

            foreach (var role in adminRoles)
            {
                foreach (var perm in toAdd)
                {
                    var exists = await _context.RolePermissions.AnyAsync(rp => rp.RoleId == role.Id && rp.PermissionId == perm.Id);
                    if (!exists)
                    {
                        await _context.RolePermissions.AddAsync(new RolePermission
                        {
                            RoleId = role.Id,
                            PermissionId = perm.Id
                        });
                    }
                }
            }

            // Asignar permisos de facturación electrónica al rol Cajero
            var cashierRoles = await _context.Roles
                .IgnoreQueryFilters()
                .Where(r => r.Name == SystemRoles.Cajero)
                .ToListAsync();

            var eiViewEmit = toAdd.Where(p =>
                p.Code == PermissionCodes.ElectronicInvoicing.View ||
                p.Code == PermissionCodes.ElectronicInvoicing.Emit).ToList();

            foreach (var role in cashierRoles)
            {
                foreach (var perm in eiViewEmit)
                {
                    var exists = await _context.RolePermissions.AnyAsync(rp => rp.RoleId == role.Id && rp.PermissionId == perm.Id);
                    if (!exists)
                    {
                        await _context.RolePermissions.AddAsync(new RolePermission
                        {
                            RoleId = role.Id,
                            PermissionId = perm.Id
                        });
                    }
                }
            }
        }
    }

    private async Task SeedScreenPermissionsAsync()
    {
        if (await _context.ScreenPermissions.AnyAsync())
            return;

        var screens = new List<ScreenPermission>
        {
            // Dashboard
            new() { ScreenCode = "dashboard", ScreenName = "Dashboard", Module = "General", Route = "/", RequiredPermission = "orders.view", DisplayOrder = 1 },

            // Operaciones
            new() { ScreenCode = "orders.list", ScreenName = "Lista de Pedidos", Module = "Operaciones", Route = "/orders", RequiredPermission = "orders.view", DisplayOrder = 10 },
            new() { ScreenCode = "orders.new", ScreenName = "Nuevo Pedido", Module = "Operaciones", Route = "/orders/new", RequiredPermission = "orders.create", DisplayOrder = 11 },
            new() { ScreenCode = "kitchen", ScreenName = "Cocina (KDS)", Module = "Operaciones", Route = "/kitchen", RequiredPermission = "kitchen.view", DisplayOrder = 20 },
            new() { ScreenCode = "tables", ScreenName = "Mesas", Module = "Operaciones", Route = "/tables", RequiredPermission = "orders.view", DisplayOrder = 30 },
            new() { ScreenCode = "billing.list", ScreenName = "Facturación", Module = "Operaciones", Route = "/billing", RequiredPermission = "billing.view", DisplayOrder = 40 },
            new() { ScreenCode = "billing.new", ScreenName = "Nueva Factura", Module = "Operaciones", Route = "/billing/new", RequiredPermission = "billing.create", DisplayOrder = 41 },
            new() { ScreenCode = "dispatch", ScreenName = "Cola de Despacho", Module = "Operaciones", Route = "/dispatch", RequiredPermission = "dispatch.view", DisplayOrder = 50 },

            // Catálogo
            new() { ScreenCode = "menu.categories", ScreenName = "Categorías", Module = "Catálogo", Route = "/menu/categories", RequiredPermission = "catalog.view", DisplayOrder = 60 },
            new() { ScreenCode = "menu.products", ScreenName = "Productos", Module = "Catálogo", Route = "/menu/products", RequiredPermission = "catalog.view", DisplayOrder = 61 },
            new() { ScreenCode = "menu.modifiers", ScreenName = "Modificadores", Module = "Catálogo", Route = "/menu/modifiers", RequiredPermission = "catalog.view", DisplayOrder = 62 },

            // Inventario
            new() { ScreenCode = "inventory", ScreenName = "Inventario", Module = "Inventario", Route = "/inventory", RequiredPermission = "catalog.view", DisplayOrder = 70 },
            new() { ScreenCode = "suppliers", ScreenName = "Proveedores", Module = "Inventario", Route = "/suppliers", RequiredPermission = "catalog.view", DisplayOrder = 71 },
            new() { ScreenCode = "purchase-orders", ScreenName = "Órdenes de Compra", Module = "Inventario", Route = "/purchase-orders", RequiredPermission = "catalog.edit", DisplayOrder = 72 },

            // Delivery
            new() { ScreenCode = "delivery.drivers", ScreenName = "Motorizados", Module = "Delivery", Route = "/delivery/drivers", RequiredPermission = "delivery.manage", DisplayOrder = 80 },
            new() { ScreenCode = "delivery.cooperatives", ScreenName = "Cooperativas", Module = "Delivery", Route = "/delivery/cooperatives", RequiredPermission = "delivery.manage", DisplayOrder = 81 },
            new() { ScreenCode = "delivery.tracking", ScreenName = "Seguimiento", Module = "Delivery", Route = "/delivery/tracking", RequiredPermission = "delivery.view", DisplayOrder = 82 },

            // Clientes
            new() { ScreenCode = "customers", ScreenName = "Clientes", Module = "Clientes", Route = "/customers", RequiredPermission = "customers.view", DisplayOrder = 90 },

            // Administración
            new() { ScreenCode = "users", ScreenName = "Usuarios", Module = "Administración", Route = "/users", RequiredPermission = "users.view", DisplayOrder = 100 },
            new() { ScreenCode = "roles", ScreenName = "Roles", Module = "Administración", Route = "/roles", RequiredPermission = "roles.view", DisplayOrder = 101 },
            new() { ScreenCode = "reports", ScreenName = "Reportes", Module = "Administración", Route = "/reports", RequiredPermission = "reports.view", DisplayOrder = 110 },
            new() { ScreenCode = "settings", ScreenName = "Configuración", Module = "Administración", Route = "/account-settings", RequiredPermission = "tenant.view", DisplayOrder = 120 },

            // QR Codes
            new() { ScreenCode = "qr-codes", ScreenName = "Códigos QR", Module = "Operaciones", Route = "/qr-codes", RequiredPermission = "orders.view", DisplayOrder = 55 },

            // IA
            new() { ScreenCode = "ai.templates", ScreenName = "Plantillas IA", Module = "IA", Route = "/ai/templates", RequiredPermission = "ai.manage", DisplayOrder = 130 },
        };

        await _context.ScreenPermissions.AddRangeAsync(screens);
        _logger.LogInformation("Pantallas-permisos creados: {Count}", screens.Count);
    }

    private async Task SeedDemoTenantAsync()
    {
        if (await _context.Tenants.AnyAsync())
            return;

        // Crear tenant de demostración
        var tenant = new Tenant
        {
            Name = "Restaurante Demo",
            Slug = "demo",
            Ruc = "9999999999001",
            IdentificationNumber = "9999999999001",
            BusinessName = "Restaurante Demo S.A.",
            Email = "demo@restaurant.com",
            Phone = "+593999999999",
            Status = TenantStatus.Active
        };

        await _context.Tenants.AddAsync(tenant);
        await _context.SaveChangesAsync();

        // Crear roles del sistema
        var adminRole = new Role
        {
            TenantId = tenant.Id,
            Name = SystemRoles.Administrador,
            NormalizedName = SystemRoles.Administrador.ToUpperInvariant(),
            Description = "Acceso completo al sistema",
            IsSystem = true
        };

        var waiterRole = new Role
        {
            TenantId = tenant.Id,
            Name = SystemRoles.Mesero,
            NormalizedName = SystemRoles.Mesero.ToUpperInvariant(),
            Description = "Gestión de mesas y pedidos",
            IsSystem = true
        };

        var kitchenRole = new Role
        {
            TenantId = tenant.Id,
            Name = SystemRoles.Cocina,
            NormalizedName = SystemRoles.Cocina.ToUpperInvariant(),
            Description = "Gestión de preparación de pedidos",
            IsSystem = true
        };

        var cashierRole = new Role
        {
            TenantId = tenant.Id,
            Name = SystemRoles.Cajero,
            NormalizedName = SystemRoles.Cajero.ToUpperInvariant(),
            Description = "Gestión de cobros y facturación",
            IsSystem = true
        };

        var marketingRole = new Role
        {
            TenantId = tenant.Id,
            Name = SystemRoles.Marketing,
            NormalizedName = SystemRoles.Marketing.ToUpperInvariant(),
            Description = "Gestión de campañas y promociones",
            IsSystem = true
        };

        var accountingRole = new Role
        {
            TenantId = tenant.Id,
            Name = SystemRoles.Contabilidad,
            NormalizedName = SystemRoles.Contabilidad.ToUpperInvariant(),
            Description = "Gestión contable y financiera",
            IsSystem = true
        };

        var managementRole = new Role
        {
            TenantId = tenant.Id,
            Name = SystemRoles.Gestion,
            NormalizedName = SystemRoles.Gestion.ToUpperInvariant(),
            Description = "Supervisión y aprobaciones gerenciales",
            IsSystem = true
        };

        var salesRole = new Role
        {
            TenantId = tenant.Id,
            Name = SystemRoles.Ventas,
            NormalizedName = SystemRoles.Ventas.ToUpperInvariant(),
            Description = "Gestión de ventas y atención al cliente",
            IsSystem = true
        };

        var payrollRole = new Role
        {
            TenantId = tenant.Id,
            Name = SystemRoles.Nomina,
            NormalizedName = SystemRoles.Nomina.ToUpperInvariant(),
            Description = "Gestión de nómina y empleados",
            IsSystem = true
        };

        await _context.Roles.AddRangeAsync(
            adminRole, waiterRole, kitchenRole, cashierRole,
            marketingRole, accountingRole, managementRole, salesRole, payrollRole);
        await _context.SaveChangesAsync();

        // Asignar todos los permisos al rol admin
        var allPermissions = await _context.Permissions.ToListAsync();
        foreach (var permission in allPermissions)
        {
            await _context.RolePermissions.AddAsync(new RolePermission
            {
                RoleId = adminRole.Id,
                PermissionId = permission.Id
            });
        }

        // Asignar permisos específicos a mesero
        var waiterPermissions = allPermissions.Where(p =>
            p.Code.StartsWith("orders.") || p.Code == "catalog.view");
        foreach (var permission in waiterPermissions)
        {
            await _context.RolePermissions.AddAsync(new RolePermission
            {
                RoleId = waiterRole.Id,
                PermissionId = permission.Id
            });
        }

        // Asignar permisos específicos a cocina
        var kitchenPermissions = allPermissions.Where(p =>
            p.Code.StartsWith("kitchen.") || p.Code == "orders.view");
        foreach (var permission in kitchenPermissions)
        {
            await _context.RolePermissions.AddAsync(new RolePermission
            {
                RoleId = kitchenRole.Id,
                PermissionId = permission.Id
            });
        }

        // Asignar permisos específicos a cajero
        var cashierPermissions = allPermissions.Where(p =>
            p.Code.StartsWith("billing.") || p.Code == "orders.view" || p.Code == "cashier.manage");
        foreach (var permission in cashierPermissions)
        {
            await _context.RolePermissions.AddAsync(new RolePermission
            {
                RoleId = cashierRole.Id,
                PermissionId = permission.Id
            });
        }

        // Asignar permisos a Marketing
        var marketingPermissions = allPermissions.Where(p =>
            p.Code.StartsWith("marketing.") || p.Code == "catalog.view" || p.Code == "ai.generate" || p.Code == "ai.manage");
        foreach (var permission in marketingPermissions)
        {
            await _context.RolePermissions.AddAsync(new RolePermission
            {
                RoleId = marketingRole.Id,
                PermissionId = permission.Id
            });
        }

        // Asignar permisos a Contabilidad
        var accountingPermissions = allPermissions.Where(p =>
            p.Code.StartsWith("accounting.") || p.Code == "billing.view" || p.Code == "reports.view" || p.Code == "reports.export");
        foreach (var permission in accountingPermissions)
        {
            await _context.RolePermissions.AddAsync(new RolePermission
            {
                RoleId = accountingRole.Id,
                PermissionId = permission.Id
            });
        }

        // Asignar permisos a Gestión
        var managementPermissions = allPermissions.Where(p =>
            p.Code.StartsWith("management.") || p.Code == "orders.view" || p.Code == "dispatch.approve" ||
            p.Code == "dispatch.view" || p.Code == "reports.view" || p.Code == "reports.export");
        foreach (var permission in managementPermissions)
        {
            await _context.RolePermissions.AddAsync(new RolePermission
            {
                RoleId = managementRole.Id,
                PermissionId = permission.Id
            });
        }

        // Asignar permisos a Ventas
        var salesPermissionsSet = allPermissions.Where(p =>
            p.Code.StartsWith("sales.") || p.Code == "orders.create" || p.Code == "orders.view" ||
            p.Code == "catalog.view" || p.Code == "customers.view");
        foreach (var permission in salesPermissionsSet)
        {
            await _context.RolePermissions.AddAsync(new RolePermission
            {
                RoleId = salesRole.Id,
                PermissionId = permission.Id
            });
        }

        // Asignar permisos a Nómina
        var payrollPermissions = allPermissions.Where(p =>
            p.Code.StartsWith("payroll.") || p.Code == "users.view");
        foreach (var permission in payrollPermissions)
        {
            await _context.RolePermissions.AddAsync(new RolePermission
            {
                RoleId = payrollRole.Id,
                PermissionId = permission.Id
            });
        }

        // Crear usuario admin
        var adminUser = new User
        {
            TenantId = tenant.Id,
            Email = "admin@demo.com",
            Username = "admin",
            PasswordHash = _passwordService.HashPassword("Admin123!"),
            FirstName = "Admin",
            LastName = "Demo",
            Status = UserStatus.Active,
            EmailVerified = true
        };

        await _context.Users.AddAsync(adminUser);
        await _context.SaveChangesAsync();

        // Asignar rol admin al usuario
        await _context.UserRoles.AddAsync(new UserRole
        {
            UserId = adminUser.Id,
            RoleId = adminRole.Id
        });

        _logger.LogInformation("Tenant demo inicializado con usuario: admin@demo.com");
    }

    private async Task SeedSuperAdminAsync()
    {
        // SuperAdmin user has TenantId = Guid.Empty (no tenant)
        var superAdminExists = await _context.Users
            .IgnoreQueryFilters()
            .AnyAsync(u => u.TenantId == SuperAdminConstants.TenantId);

        if (superAdminExists)
            return;

        // Ensure system tenant exists for FK constraint (Guid.Empty)
        var systemTenantExists = await _context.Tenants
            .IgnoreQueryFilters()
            .AnyAsync(t => t.Id == SuperAdminConstants.TenantId);

        if (!systemTenantExists)
        {
            // Raw SQL required because EF auto-generates Guid.Empty (ValueGeneratedOnAdd)
            await _context.Database.ExecuteSqlRawAsync(
                @"INSERT INTO ""Tenants"" (""Id"", ""Name"", ""Slug"", ""BusinessName"", ""Email"", ""PrimaryColor"", ""SecondaryColor"", ""Currency"", ""TaxPercentage"", ""TimeZone"", ""Status"", ""DeliveryOperationMode"", ""OnboardingCompleted"", ""CreatedAt"")
                  VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13})",
                SuperAdminConstants.TenantId, "Sistema", "system", "RestoSaaS Platform", "system@restosaas.com",
                "#1976D2", "#FF9800", "USD", 15.00m, "America/Guayaquil",
                (int)TenantStatus.Active, (int)DeliveryOperationMode.Standalone, true, DateTime.UtcNow);
        }

        // Create SuperAdmin role (TenantId = Guid.Empty)
        var superAdminRole = new Role
        {
            TenantId = SuperAdminConstants.TenantId,
            Name = SystemRoles.SuperAdmin,
            NormalizedName = SystemRoles.SuperAdmin.ToUpperInvariant(),
            Description = "Administrador global de la plataforma RestoSaaS",
            IsSystem = true
        };

        await _context.Roles.AddAsync(superAdminRole);
        await _context.SaveChangesAsync();

        // Assign all permissions to SuperAdmin role
        var allPermissions = await _context.Permissions.ToListAsync();
        foreach (var permission in allPermissions)
        {
            await _context.RolePermissions.AddAsync(new RolePermission
            {
                RoleId = superAdminRole.Id,
                PermissionId = permission.Id
            });
        }

        // Create SuperAdmin user
        var superAdminUser = new User
        {
            TenantId = SuperAdminConstants.TenantId,
            Email = "superadmin@restosaas.com",
            PasswordHash = _passwordService.HashPassword("SuperAdmin123!"),
            FirstName = "Super",
            LastName = "Admin",
            Status = UserStatus.Active,
            EmailVerified = true
        };

        await _context.Users.AddAsync(superAdminUser);
        await _context.SaveChangesAsync();

        // Assign SuperAdmin role
        await _context.UserRoles.AddAsync(new UserRole
        {
            UserId = superAdminUser.Id,
            RoleId = superAdminRole.Id
        });

        _logger.LogInformation("SuperAdmin inicializado: superadmin@restosaas.com");
    }

    /// <summary>
    /// Siembra los planes de suscripcion base. Idempotente.
    /// </summary>
    private async Task SeedPlansAsync()
    {
        if (await _context.Plans.AnyAsync())
            return;

        // Pantallas del plan Basico
        var basicScreens = new List<string>
        {
            "dashboard", "orders.list", "orders.new", "kitchen", "billing.list", "billing.new", "tables"
        };

        // Pantallas del plan Pro (todo el basico + mas)
        var proScreens = new List<string>(basicScreens)
        {
            "delivery.drivers", "delivery.cooperatives", "delivery.tracking",
            "reports", "menu.categories", "menu.products", "menu.modifiers",
            "inventory", "suppliers", "purchase-orders", "qr-codes", "dispatch",
            "customers"
        };

        // Pantallas del plan Enterprise (todo lo disponible)
        var enterpriseScreens = new List<string>(proScreens)
        {
            "users", "roles", "settings", "ai.templates"
        };

        var basicPlan = new Domain.Entities.Plan
        {
            Name = "Básico",
            Description = "Plan ideal para restaurantes pequeños. Incluye módulos esenciales de operación.",
            MonthlyPrice = 49m,
            AnnualPrice = 470m,
            MaxUsers = 3,
            MaxBranches = 1,
            IsActive = true,
            IsDefault = true,
            DisplayOrder = 1,
            Color = "#4CAF50"
        };

        foreach (var code in basicScreens)
            basicPlan.PlanScreenPermissions.Add(new Domain.Entities.PlanScreenPermission { ScreenCode = code, IsIncluded = true });

        var proPlan = new Domain.Entities.Plan
        {
            Name = "Pro",
            Description = "Plan completo para restaurantes en crecimiento. Delivery, reportes, inventario y más.",
            MonthlyPrice = 99m,
            AnnualPrice = 950m,
            MaxUsers = 10,
            MaxBranches = 3,
            IsActive = true,
            IsDefault = false,
            DisplayOrder = 2,
            Color = "#C8963E"
        };

        foreach (var code in proScreens)
            proPlan.PlanScreenPermissions.Add(new Domain.Entities.PlanScreenPermission { ScreenCode = code, IsIncluded = true });

        var enterprisePlan = new Domain.Entities.Plan
        {
            Name = "Enterprise",
            Description = "Plan sin límites para cadenas y franquicias. Usuarios ilimitados, sucursales ilimitadas, IA.",
            MonthlyPrice = 199m,
            AnnualPrice = 1910m,
            MaxUsers = 0,
            MaxBranches = 0,
            IsActive = true,
            IsDefault = false,
            DisplayOrder = 3,
            Color = "#9C27B0"
        };

        foreach (var code in enterpriseScreens)
            enterprisePlan.PlanScreenPermissions.Add(new Domain.Entities.PlanScreenPermission { ScreenCode = code, IsIncluded = true });

        await _context.Plans.AddRangeAsync(basicPlan, proPlan, enterprisePlan);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Planes de suscripción creados: Básico, Pro, Enterprise");
    }

    /// <summary>
    /// Siembra los parametros de configuracion global del sistema (GeneralValues).
    /// Idempotente: solo agrega claves que no existan aun.
    /// </summary>
    private async Task SeedGeneralValuesAsync()
    {
        var existingKeys = await _context.GeneralValues
            .Select(v => v.Key)
            .ToListAsync();

        if (existingKeys.Count > 0)
        {
            _logger.LogInformation("GeneralValues ya sembrados ({Count} existentes), omitiendo", existingKeys.Count);
            return;
        }

        // Categorias raiz (agrupadores)
        var configParent = GeneralValue.Create("CONFIG", "Configuracion General", null, GeneralValueType.String, "CONFIG",
            "Parametros globales de la aplicacion", null, false, 1);
        var securityParent = GeneralValue.Create("SECURITY", "Seguridad", null, GeneralValueType.String, "SECURITY",
            "Parametros de seguridad y autenticacion", null, false, 2);
        var limitsParent = GeneralValue.Create("LIMITS", "Limites del Sistema", null, GeneralValueType.String, "LIMITS",
            "Limites numericos y cuotas de uso", null, false, 3);

        await _context.GeneralValues.AddRangeAsync(configParent, securityParent, limitsParent);
        await _context.SaveChangesAsync();

        // Hijos de CONFIG
        var configValues = new[]
        {
            GeneralValue.Create("CONFIG/MAX_LOGIN_ATTEMPTS", "Intentos Maximos de Login", "5",
                GeneralValueType.Number, "CONFIG", "Cantidad de intentos fallidos antes de bloquear la cuenta",
                configParent.Id, true, 1),
            GeneralValue.Create("CONFIG/MAINTENANCE_MODE", "Modo Mantenimiento", "false",
                GeneralValueType.Boolean, "CONFIG", "Si es true, bloquea acceso a usuarios no administradores",
                configParent.Id, true, 2),
            GeneralValue.Create("CONFIG/SUPPORT_EMAIL", "Email de Soporte", "soporte@restify.com",
                GeneralValueType.String, "CONFIG", "Correo de contacto mostrado en errores y footer",
                configParent.Id, true, 3),
        };

        // Hijos de SECURITY
        var securityValues = new[]
        {
            GeneralValue.Create("SECURITY/JWT_ACCESS_EXPIRY_MINUTES", "Expiracion Access Token (min)", "60",
                GeneralValueType.Number, "SECURITY", "Tiempo de vida del access token JWT en minutos",
                securityParent.Id, true, 1),
            GeneralValue.Create("SECURITY/JWT_REFRESH_EXPIRY_DAYS", "Expiracion Refresh Token (dias)", "7",
                GeneralValueType.Number, "SECURITY", "Tiempo de vida del refresh token JWT en dias",
                securityParent.Id, true, 2),
            GeneralValue.Create("SECURITY/BCRYPT_WORK_FACTOR", "Factor de Trabajo BCrypt", "12",
                GeneralValueType.Number, "SECURITY", "Factor de costo de hashing. Minimo 12 segun politica de seguridad",
                securityParent.Id, false, 3),
            GeneralValue.Create("SECURITY/MAX_FAILED_LOGIN_ATTEMPTS", "Intentos Fallidos Maximos", "5",
                GeneralValueType.Number, "SECURITY", "Numero de intentos fallidos antes de bloquear la cuenta",
                securityParent.Id, true, 4),
            GeneralValue.Create("SECURITY/LOCKOUT_DURATION_MINUTES", "Duracion Bloqueo (min)", "30",
                GeneralValueType.Number, "SECURITY", "Duracion del bloqueo de cuenta tras intentos fallidos",
                securityParent.Id, true, 5),
        };

        // Hijos de LIMITS
        var limitsValues = new[]
        {
            GeneralValue.Create("LIMITS/MAX_PAGINATION_SIZE", "Tamano Maximo de Pagina", "100",
                GeneralValueType.Number, "LIMITS", "Maximo de registros por pagina en listados",
                limitsParent.Id, true, 1),
            GeneralValue.Create("LIMITS/DEFAULT_PAGE_SIZE", "Tamano de Pagina por Defecto", "20",
                GeneralValueType.Number, "LIMITS", "Registros por defecto si no se especifica paginacion",
                limitsParent.Id, true, 2),
        };

        await _context.GeneralValues.AddRangeAsync(configValues);
        await _context.GeneralValues.AddRangeAsync(securityValues);
        await _context.GeneralValues.AddRangeAsync(limitsValues);
        await _context.SaveChangesAsync();

        _logger.LogInformation("GeneralValues sembrados: {Count} registros",
            3 + configValues.Length + securityValues.Length + limitsValues.Length);
    }
}
