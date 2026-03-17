---
description: Validar entidades y logica de negocio del BackOffice - verificar validators, mappings, servicios
---

# Skill: backoffice-validate — Validacion de BackOffice

Ejecuta una validacion del servicio BackOffice verificando la integridad de validators, mappings y servicios.

## Pasos

### 1. Verificar Validators
Busca todos los validators y verifica que cada entidad principal tiene sus validators:
```bash
find BackEnd/Restify.BackOffice -path "*/Validators/*.cs" | grep -v bin | grep -v obj | sort
```

Entidades que DEBEN tener validators:
- Category, Product, Supplier, GlobalModifier, Table
- Order, Invoice, CashRegister
- InventoryItem, PurchaseOrder
- Employee, Payroll, Customer

### 2. Verificar Mapping Extensions
```bash
find BackEnd/Restify.BackOffice -path "*/Extensions/*Mapping*" -o -path "*/Extensions/*Extension*" | grep -v bin | grep -v obj | sort
```

Cada entidad debe tener: `.ToDto()`, `.ToEntity()`, `.UpdateFrom()`

### 3. Verificar Servicios
```bash
find BackEnd/Restify.BackOffice -path "*/Services/*.cs" -o -path "*/Interfaces/I*Service.cs" | grep -v bin | grep -v obj | sort
```

### 4. Compilar
```bash
cd BackEnd/Restify.BackOffice && "/mnt/c/Program Files/dotnet/dotnet.exe" build 2>&1
```

### 5. Reporte
```
## Validacion BackOffice

| Componente | Esperado | Encontrado | Estado |
|------------|----------|------------|--------|
| Validators | 14 archivos | N | OK/FALLO |
| Mappings | 8 archivos | N | OK/FALLO |
| Servicios | N interfaces | N impl | OK/FALLO |
| Build | 0 errores | N | OK/FALLO |
```
