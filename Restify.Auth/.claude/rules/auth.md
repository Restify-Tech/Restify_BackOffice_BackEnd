# Reglas — Restify.Auth Service

## Dominio
Este servicio maneja autenticacion, usuarios, roles, permisos y registro de tenants.

## Entidades Principales
- **Tenant**: Organizacion (restaurante), con config de delivery, fiscal, onboarding
- **User**: Usuario del sistema, scoped por TenantId
- **Role**: Rol asignable a usuarios, scoped por TenantId
- **Permission**: Permiso del sistema, con IsTenantDefault flag
- **UserRole / RolePermission**: Tablas de relacion

## Clean Architecture
```
Restify.Auth/
├── src/
│   ├── Restify.Auth.Domain/         # Entidades, Constants/, enums
│   ├── Restify.Auth.Application/    # DTOs, interfaces, validators
│   ├── Restify.Auth.Infrastructure/ # AuthDbContext, repos, services
│   └── Restify.Auth.Api/            # Controllers, Program.cs
```

## Constantes del Sistema (Domain/Constants/)
- `ClaimConstants.cs` — Nombres de claims JWT
- `UserTypes.cs` — superadmin, admin, employee, customer, pool_driver
- `SystemRoles.cs` — Administrador, Mesero, Cocina, Cajero, Delivery, etc.
- `PermissionCodes.cs` — Todos los codigos de permisos por modulo
- `SuperAdminConstants.cs` — Email, TenantId=Guid.Empty

## Seguridad
- JWT con SecretKey >= 32 caracteres (compartida entre servicios)
- BCrypt factor 12 para passwords
- RefreshToken con rotacion y revocacion
- SuperAdmin: TenantId == Guid.Empty, claim is_superadmin=true
- Login superadmin: superadmin@restosaas.com / SuperAdmin123!

## Endpoints
- `/api/auth` — login, refresh, logout, me, change-password
- `/api/users` — CRUD usuarios (paginado)
- `/api/roles` — CRUD roles + permisos
- `/api/tenants` — Gestion de tenants (SuperAdmin)
- `/api/tenants/register` — Registro publico de tenant
- `/api/pool-drivers` — Registro/login drivers del pool

## Build
```bash
cd BackEnd/Restify.Auth && "/mnt/c/Program Files/dotnet/dotnet.exe" build
```

## Schema: `public` (default PostgreSQL)
