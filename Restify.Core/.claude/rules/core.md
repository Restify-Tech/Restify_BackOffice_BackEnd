# Reglas — Restify.Core Service

## Dominio
Este servicio maneja configuraciones de grids, tablas generales y valores parametricos del sistema.

## Entidades Principales
- **GridConfiguration**: Configuracion de columnas y comportamiento de grids
- **GridColumn**: Columna individual de un grid
- **GeneralTable**: Tabla parametrica (paises, tipos de documento, etc.)
- **GeneralValue**: Valor dentro de una tabla general

## Clean Architecture
```
Restify.Core/
├── src/
│   ├── Restify.Core.Domain/         # Entidades, Constants/
│   ├── Restify.Core.Application/    # DTOs, interfaces
│   ├── Restify.Core.Infrastructure/ # CoreDbContext, repos
│   └── Restify.Core.Api/            # Controllers, Program.cs
```

## Schema: `core`
Todas las tablas van en el schema `core` de PostgreSQL.

## JWT Compartido
Usa la misma configuracion JWT que Auth (SecretKey, Issuer, Audience identicos).
ClaimConstants copiado localmente en `Core.Domain/Constants/`.

## API: http://localhost:55966

## Build
```bash
cd BackEnd/Restify.Core && "/mnt/c/Program Files/dotnet/dotnet.exe" build
```
