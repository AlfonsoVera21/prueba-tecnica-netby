# Prueba técnica - Backend .NET (Microservicios de productos y transacciones)

Este repositorio contiene dos microservicios minimal API en .NET 8 que cumplen con los requisitos funcionales de la prueba:

- **Gestión de productos**: creación, edición, consulta y eliminación con validación de stock.
- **Gestión de transacciones**: registra compras y ventas actualizando el stock disponible.
- **Búsquedas y filtrados**: se aplican filtros por nombre, código, categoría, tipo y rango de precios usando búsquedas binarias para resolver coincidencias por prefijo de nombre o código de forma eficiente.
- **Historial de movimientos**: se exponen transacciones ordenadas por fecha y el historial por producto.

La persistencia utiliza SQLite y se incluye el script `database.sql` en la raíz para crear las tablas requeridas.

## Requisitos

- .NET SDK 8.0+
- SQLite (el runtime de `Microsoft.Data.Sqlite` se encarga de crear el archivo si no existe)

## Estructura

```
src/
  SharedKernel/        // Entidades compartidas, enumeraciones y utilidades de búsqueda binaria
  ProductService/      // API REST para productos y búsquedas
  TransactionService/  // API REST para transacciones y validación de stock
```

## Configuración rápida

Ambos servicios comparten el mismo archivo `data/netby.db`. Si se ejecutan en el mismo host pueden iniciarse en puertos diferentes. A modo de ejemplo:

```bash
cd src/ProductService
# dotnet restore
# dotnet run --urls http://localhost:5080

cd ../TransactionService
# dotnet restore
# dotnet run --urls http://localhost:5081
```

> Nota: en el entorno de evaluación automático no está disponible el SDK de .NET para ejecutar los comandos anteriores.

Swagger queda habilitado en `Development` y muestra los endpoints disponibles.

## Endpoints principales

### ProductService
- `GET /products` Filtra por `name`, `code`, `category`, `type`, `minPrice`, `maxPrice` utilizando búsqueda binaria para nombres y códigos.
- `GET /products/{code}` Obtiene un producto.
- `POST /products` Crea un nuevo producto.
- `PUT /products/{code}` Actualiza un producto existente.
- `DELETE /products/{code}` Elimina un producto.
- `POST /products/{code}/stock` Ajusta el stock asegurando que no quede negativo.

### TransactionService
- `GET /transactions` Filtra transacciones por `productCode`, tipo y rango de fechas.
- `POST /transactions` Registra compras o ventas y actualiza el stock (valida disponibilidad en ventas).
- `GET /transactions/products/{code}` Devuelve el historial de transacciones y stock actual para un producto.

## Script SQL

Ejecuta `database.sql` para crear las tablas necesarias en SQLite:

```bash
sqlite3 data/netby.db < database.sql
```

## Criterios de aceptación cubiertos

- APIs REST para CRUD de productos y manejo de transacciones.
- Validación de stock en ventas y actualización en cada movimiento.
- Búsquedas con filtros múltiples y uso explícito de búsqueda binaria para coincidencias por prefijo.
- Tablas separadas para productos y transacciones.
- Script SQL y pasos de ejecución documentados.
