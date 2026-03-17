# Reglas — Restify.BackOffice Service

## Dominio
Este servicio maneja la operacion del restaurante: menu, pedidos, mesas, facturacion, inventario, delivery, empleados, contabilidad.

## Entidades Principales
- **Category / Product / GlobalModifier**: Menu y catalogo
- **Table / Order / OrderDetail**: Mesas y pedidos
- **Invoice / CashRegister**: Facturacion y caja
- **Supplier / InventoryItem / PurchaseOrder**: Inventario y compras
- **DeliveryDriver / Delivery**: Delivery y GPS
- **Employee / Payroll**: Recursos humanos
- **Customer**: Clientes del restaurante

## Clean Architecture
```
Restify.BackOffice/
├── src/
│   ├── Restify.BackOffice.Domain/         # Entidades, enums
│   ├── Restify.BackOffice.Application/    # DTOs, interfaces, 34 validators
│   ├── Restify.BackOffice.Infrastructure/ # BackOfficeDbContext, 11 repos
│   └── Restify.BackOffice.Api/            # Controllers
```

## Schema: `backoffice`
Todas las tablas en schema `backoffice` de PostgreSQL.

## Validators
34 validators en 14 archivos bajo `Application/Validators/`:
Category, Product, Supplier, GlobalModifier, Table, Order, Invoice, CashRegister, Inventory, PurchaseOrder, Accounting, Employee, Payroll, Customer.

## Mapping Extensions
8 archivos de mapping en `Application/Extensions/`: `.ToDto()`, `.ToEntity()`, `.UpdateFrom()`

## Delivery
- DeliveryDriver con IsPoolDriver, GPS fields, VerificationStatus
- Delivery con IsPoolDelivery, PoolCommission
- SignalR Hub: `/hubs/delivery-tracking`

## Build
```bash
cd BackEnd/Restify.BackOffice && "/mnt/c/Program Files/dotnet/dotnet.exe" build
```
