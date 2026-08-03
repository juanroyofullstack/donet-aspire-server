# API

This document describes the current HTTP surface exposed by the API project.

## Overview

The API is implemented as a Minimal API with a composition-root entry point in `src/Api/Program.cs` and endpoint mappings split by vertical in `src/Api/Endpoints/`.

The product endpoints delegate to `ProductService`, which in turn uses the repository abstraction selected at startup.

For architectural context, see [architecture.md](architecture.md).

## Base Behavior

- System endpoints are mapped through `MapSystemEndpoints()`.
- Product endpoints are mapped through `MapProductEndpoints()`.
- Product payloads use explicit API contracts: `CreateProductRequest` and `ProductResponse`.
- OpenAPI is enabled in development mode.

## Endpoint Organization

- `src/Api/Program.cs` registers services and invokes endpoint mapping extension methods.
- `src/Api/Endpoints/SystemEndpoints.cs` contains `GET /` and `GET /health`.
- `src/Api/Endpoints/ProductEndpoints.cs` contains `GET /products` and `POST /products`.
- Product endpoints include OpenAPI metadata (`WithName`, `WithTags`, `WithSummary`, `WithDescription`, `Produces`).

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

- Uses `ProductService.GetAllAsync(cancellationToken)`.
- Maps domain entities to `ProductResponse`.
- Returns `200 OK`.

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
- `ProductService.CreateAsync(name, price, cancellationToken)` creates the domain entity.
- The repository persists the entity.
- The response payload is `ProductResponse`.
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

- [src/Api/Program.cs](../../src/Api/Program.cs)
- [src/Api/Endpoints/SystemEndpoints.cs](../../src/Api/Endpoints/SystemEndpoints.cs)
- [src/Api/Endpoints/ProductEndpoints.cs](../../src/Api/Endpoints/ProductEndpoints.cs)
- [src/Api/Contracts/Products/CreateProductRequest.cs](../../src/Api/Contracts/Products/CreateProductRequest.cs)
- [src/Api/Contracts/Products/ProductResponse.cs](../../src/Api/Contracts/Products/ProductResponse.cs)
- [architecture.md](architecture.md)
- [configuration.md](configuration.md)
