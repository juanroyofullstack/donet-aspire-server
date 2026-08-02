# LLM Context

This document is a compact reference for understanding the repository quickly. It is optimized for LLM consumption: short sections, explicit facts, and minimal narrative.

## Project Summary

NetAspireServer is a .NET Aspire-based API that exposes a small product service. The code is organized using a clean layered structure with `Api`, `Application`, `Domain`, and `Infrastructure` projects.

The system supports two persistence modes:

- Cosmos DB when the `CosmosDb` configuration section is complete.
- In-memory storage when Cosmos is not configured.

## Main Entry Points

- [src/Api/Program.cs](../../src/Api/Program.cs): API bootstrap, dependency injection, and endpoint mapping.
- [src/AppHost/AppHost.cs](../../src/AppHost/AppHost.cs): Aspire orchestration host.
- [docs/reference/architecture.md](../reference/architecture.md): full architecture reference.

## Layer Map

### Api

- Hosts the HTTP endpoints.
- Registers services.
- Chooses the repository implementation at startup.

### Application

- Contains `ProductService`.
- Defines `IProductRepository`.
- Keeps use-case logic independent from storage details.

### Domain

- Contains `Product`.
- Enforces core invariants in the entity constructor.

### Infrastructure

- Contains `CosmosProductRepository`.
- Contains `InMemoryProductRepository`.
- Contains `CosmosDbOptions`.

## Runtime Flow

1. `Program.cs` builds the web app.
2. DI registers `IProductRepository`.
3. The app resolves either Cosmos-backed or in-memory persistence.
4. Endpoints call `ProductService`.
5. `ProductService` creates or reads `Product` entities.
6. The repository persists or returns the data.

## Repository Selection Rules

- If `CosmosDbOptions.IsConfigured` is `true`, use `CosmosProductRepository`.
- Otherwise, use `InMemoryProductRepository`.

This fallback makes the API runnable without external infrastructure.

## Domain Rules

`Product` validates its own data:

- `Name` must not be null, empty, or whitespace.
- `Price` must not be negative.

If these rules fail, the constructor throws before persistence happens.

## API Surface

The current API endpoints are:

- `GET /` returns a basic status response.
- `GET /health` returns a health status.
- `GET /products` returns all products.
- `POST /products` creates a product.

### POST /products

Request body:

```json
{
  "name": "Laptop",
  "price": 999.99
}
```

Response:

- `201 Created` on success.
- Location header points to `/products/{id}`.

## Configuration

Relevant settings:

- `CosmosDb:ConnectionString`
- `CosmosDb:DatabaseName`
- `CosmosDb:ContainerName`

When these values are complete, the Cosmos repository is used. If not, the app falls back to in-memory storage.

`src/Api/appsettings.json` currently contains only logging and allowed hosts settings. `src/Api/appsettings.Development.json` sets development logging verbosity.

## Aspire Notes

`AppHost` adds `ASPIRE_ALLOW_UNSECURED_TRANSPORT = true` for local development and registers the API project so Aspire can launch it.

## Useful File References

- [src/Api/Program.cs](../../src/Api/Program.cs)
- [src/Application/Services/ProductService.cs](../../src/Application/Services/ProductService.cs)
- [src/Application/Interfaces/IProductRepository.cs](../../src/Application/Interfaces/IProductRepository.cs)
- [src/Domain/Entities/Product.cs](../../src/Domain/Entities/Product.cs)
- [src/Infrastructure/Repositories/CosmosProductRepository.cs](../../src/Infrastructure/Repositories/CosmosProductRepository.cs)
- [src/Infrastructure/Repositories/InMemoryProductRepository.cs](../../src/Infrastructure/Repositories/InMemoryProductRepository.cs)
- [src/Infrastructure/Configuration/CosmosDbOptions.cs](../../src/Infrastructure/Configuration/CosmosDbOptions.cs)