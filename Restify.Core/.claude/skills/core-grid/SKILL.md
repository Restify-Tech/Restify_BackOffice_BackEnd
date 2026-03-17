---
description: Crear o modificar configuraciones de grid - columnas, filtros, ordenamiento, paginacion
---

# Skill: core-grid — Gestion de Grid Configurations

Ayuda a crear o modificar configuraciones de grid para el sistema.

## Pasos

### 1. Listar grids existentes
Lee las configuraciones actuales en el proyecto Core:
```bash
find BackEnd/Restify.Core -path "*/GridConfiguration*" -o -path "*/GridColumn*" | grep -v bin | grep -v obj
```

### 2. Si el usuario quiere crear un nuevo grid
Necesita:
- **Nombre del grid** (ej: "users-list", "products-list")
- **Columnas**: field, headerName, type, width, sortable, filterable
- **Configuracion**: pageSize, defaultSort, defaultFilter

### 3. Generar la configuracion
Crear el seed o endpoint que registre el GridConfiguration con sus GridColumns.

### 4. Verificar
- Compilar: `"/mnt/c/Program Files/dotnet/dotnet.exe" build`
- Verificar que el frontend consume el grid via `useGridConfig` hook
