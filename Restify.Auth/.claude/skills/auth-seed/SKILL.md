---
description: Gestionar el seeder de datos iniciales de Auth - superadmin, permisos, roles base, tenant demo
---

# Skill: auth-seed — Gestion del Seeder Auth

Revisa y gestiona el seeder de datos iniciales del servicio Auth.

## Pasos

### 1. Leer el seeder actual
Busca el archivo DbSeeder o DataSeeder en el proyecto Auth:
```bash
find BackEnd/Restify.Auth -name "*Seeder*" -o -name "*Seed*" | grep -v bin | grep -v obj
```

### 2. Verificar que incluye

**SuperAdmin (obligatorio):**
- Email: superadmin@restosaas.com
- Password: SuperAdmin123! (hasheado con BCrypt)
- TenantId: Guid.Empty
- UserType: superadmin

**Permisos del sistema:**
- Todos los permisos definidos en PermissionCodes.cs
- Flag IsTenantDefault correcto para cada permiso

**Tenant Demo (Development only):**
- Email: admin@demo.com
- Password: Admin123!
- Rol: Administrador con todos los permisos tenant

### 3. Reportar estado
Genera un reporte de lo que esta y lo que falta en el seeder.

### 4. Si el usuario pide cambios
- Modificar el seeder siguiendo los patrones existentes
- Solo ejecutar en ambiente Development
- Compilar despues de cambios: `"/mnt/c/Program Files/dotnet/dotnet.exe" build`
