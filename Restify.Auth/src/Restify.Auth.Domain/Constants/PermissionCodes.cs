namespace Restify.Auth.Domain.Constants;

/// <summary>
/// Códigos de permisos del sistema, organizados por módulo.
/// Sincronizado 1:1 con los permisos en DbSeeder.
/// </summary>
public static class PermissionCodes
{
    public static class Auth
    {
        public const string Manage = "auth.manage";
    }

    public static class Users
    {
        public const string View = "users.view";
        public const string Create = "users.create";
        public const string Edit = "users.edit";
        public const string Delete = "users.delete";
    }

    public static class Roles
    {
        public const string View = "roles.view";
        public const string Create = "roles.create";
        public const string Edit = "roles.edit";
        public const string Delete = "roles.delete";
    }

    public static class Tenant
    {
        public const string View = "tenant.view";
        public const string Edit = "tenant.edit";
    }

    public static class Catalog
    {
        public const string View = "catalog.view";
        public const string Create = "catalog.create";
        public const string Edit = "catalog.edit";
        public const string Delete = "catalog.delete";
    }

    public static class Orders
    {
        public const string View = "orders.view";
        public const string Create = "orders.create";
        public const string Edit = "orders.edit";
        public const string Approve = "orders.approve";
        public const string Cancel = "orders.cancel";
    }

    public static class Kitchen
    {
        public const string View = "kitchen.view";
        public const string Manage = "kitchen.manage";
    }

    public static class Billing
    {
        public const string View = "billing.view";
        public const string Create = "billing.create";
        public const string Void = "billing.void";
        public const string CashierManage = "cashier.manage";
    }

    public static class Reports
    {
        public const string View = "reports.view";
        public const string Export = "reports.export";
    }

    public static class Delivery
    {
        public const string View = "delivery.view";
        public const string Manage = "delivery.manage";
        public const string Assign = "delivery.assign";
    }

    public static class Marketing
    {
        public const string View = "marketing.view";
        public const string Create = "marketing.create";
        public const string Edit = "marketing.edit";
        public const string Delete = "marketing.delete";
    }

    public static class Accounting
    {
        public const string View = "accounting.view";
        public const string Create = "accounting.create";
        public const string Reports = "accounting.reports";
    }

    public static class Management
    {
        public const string View = "management.view";
        public const string Approve = "management.approve";
        public const string Settings = "management.settings";
    }

    public static class Sales
    {
        public const string View = "sales.view";
        public const string Create = "sales.create";
        public const string Reports = "sales.reports";
    }

    public static class Payroll
    {
        public const string View = "payroll.view";
        public const string Create = "payroll.create";
        public const string Edit = "payroll.edit";
        public const string Reports = "payroll.reports";
    }

    public static class Dispatch
    {
        public const string View = "dispatch.view";
        public const string Approve = "dispatch.approve";
        public const string Manage = "dispatch.manage";
    }

    public static class Customers
    {
        public const string View = "customers.view";
        public const string Create = "customers.create";
        public const string Edit = "customers.edit";
        public const string Delete = "customers.delete";
    }

    public static class AI
    {
        public const string Generate = "ai.generate";
        public const string Manage = "ai.manage";
    }

    public static class DeliveryZones
    {
        public const string View = "deliveryzones.view";
        public const string Create = "deliveryzones.create";
        public const string Edit = "deliveryzones.edit";
        public const string Delete = "deliveryzones.delete";
    }

    public static class Tenants
    {
        public const string View = "tenants.view";
        public const string Edit = "tenants.edit";
        public const string Manage = "tenants.manage";
    }

    public static class PoolDrivers
    {
        public const string View = "pool_drivers.view";
        public const string Manage = "pool_drivers.manage";
        public const string Verify = "pool_drivers.verify";
    }

    public static class Commissions
    {
        public const string View = "commissions.view";
        public const string Manage = "commissions.manage";
    }

    public static class ElectronicInvoicing
    {
        public const string View = "electronic_invoicing.view";
        public const string Emit = "electronic_invoicing.emit";
        public const string Void = "electronic_invoicing.void";
        public const string Configure = "electronic_invoicing.configure";
    }
}
