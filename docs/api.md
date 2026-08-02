# API

This document describes the current HTTP surface exposed by the API project.

## Overview

The API is implemented as a Minimal API in `src/Api/Program.cs`. It exposes a small set of endpoints for health checks and product management.

The product endpoints delegate to `ProductService`, which in turn uses the repository abstraction selected at startup.

For architectural context, see [architecture.md](architecture.md).

## Base Behavior

- The API maps a root status endpoint.
- Health checks are exposed at `/health`.
- Product data is available through `/products`.
- OpenAPI is enabled in development mode.

## Endpoints

### `GET /`

Returns a simple status payload.

Response:

```json
{
  "status": "ok",
  "message": "NetAspireServer API is running."
}
```

### `GET /health`

Returns a lightweight health response.

Response:

```json
{
  "status": "ok"
}
```

### `GET /products`

Returns all products currently stored by the active repository implementation.

Behavior:

- Uses `ProductService.GetAllAsync()`.
- Returns the collection as JSON.

Example response:

```json
[
  {
    "id": "2e4f8ad7-0a73-4f84-bc5b-7ef0e8db7b3b",
    "name": "Laptop",
    "price": 999.99
  }
]
```

### `POST /products`

Creates a new product.

Request body:

```json
{
  "name": "Laptop",
  "price": 999.99
}
```

Request model:

- `name`: required string.
- `price`: required decimal.

Behavior:

- The endpoint binds the request to `CreateProductRequest`.
- `ProductService.CreateAsync(name, price)` creates the domain entity.
- The repository persists the entity.
- The response is `201 Created`.
- The `Location` header points to `/products/{id}`.

Example response:

```json
{
  "id": "2e4f8ad7-0a73-4f84-bc5b-7ef0e8db7b3b",
  "name": "Laptop",
  "price": 999.99
}
```

## OpenAPI

OpenAPI support is enabled through `AddOpenApi()` and exposed in development with `MapOpenApi()`.

- Development only: the generated OpenAPI document is mapped by `MapOpenApi()`.
- Production: the OpenAPI endpoint is not mapped by default.

## Error Behavior

The current API does not define custom error response types in `Program.cs`.

Relevant failures can still occur from the domain and infrastructure layers:

- Invalid product data can trigger exceptions from `Product`.
- Missing Cosmos configuration causes the app to use the in-memory repository instead of failing startup.

## Related Files

- [../src/Api/Program.cs](../src/Api/Program.cs)
- [architecture.md](architecture.md)
- [configuration.md](configuration.md)
