# Architecture

This repository uses a clean, layered .NET structure with a small API surface and the business logic kept outside the web layer. The goal is to keep the API thin, the domain rules explicit, and persistence swappable between Cosmos DB and an in-memory fallback.

## Solution Layers

The solution is split into four main projects:

- `Api`: hosts the HTTP endpoints, configures dependency injection, and wires the application together.
- `Application`: contains use-case orchestration and repository contracts.
- `Domain`: contains the core business entity and its invariants.
- `Infrastructure`: contains concrete persistence implementations and infrastructure configuration.

This separation keeps the web layer from depending directly on a storage technology and makes the product behavior easier to test and evolve.

## Request Flow

The entry point is `src/Api/Program.cs`. It does three jobs:

1. Registers services in the container.
2. Builds the web application.
3. Maps the HTTP endpoints.

The runtime flow for `GET /products` and `POST /products` is:

1. The endpoint in `Program.cs` receives the request.
2. The endpoint calls `ProductService` from the `Application` layer.
3. `ProductService` creates or retrieves `Product` entities.
4. `ProductService` delegates persistence to `IProductRepository`.
5. The container resolves either the Cosmos-backed repository or the in-memory repository.

## Application Layer

The `Application` project defines the use-case boundary for products.

- `IProductRepository` defines the persistence contract.
- `ProductService` implements the product use cases by coordinating entity creation and repository calls.

This layer does not know whether data ends up in Cosmos DB or memory. It only depends on the abstraction.

## Domain Layer

The `Domain` project owns the core business entity:

- `Product` has an `Id`, `Name`, and `Price`.
- The constructor enforces the basic rules: `Name` must not be blank and `Price` must not be negative.

Those rules are enforced before any persistence logic runs, so invalid products are rejected at the domain boundary.

## Infrastructure Layer

The `Infrastructure` project provides the concrete repository implementations and configuration binding.

- `CosmosDbOptions` binds the `CosmosDb` configuration section and exposes `IsConfigured`.
- `CosmosProductRepository` uses `Microsoft.Azure.Cosmos` to store and read products.
- `InMemoryProductRepository` stores products in a local list for development or when Cosmos is not configured.

The API selects the repository implementation at startup based on `CosmosDbOptions.IsConfigured`.

### Repository Selection

The registration in `Program.cs` resolves `IProductRepository` with this rule:

- If `CosmosDbOptions.IsConfigured` is `true`, the app uses `CosmosProductRepository`.
- Otherwise, the app falls back to `InMemoryProductRepository`.

This makes local development work without Cosmos DB configuration while still supporting a real database when the required settings are present.

### Cosmos Repository Behavior

`CosmosProductRepository` validates that the Cosmos settings are complete before it is used. When it needs storage access, it lazily creates the database and container if they do not exist yet.

Important details:

- The container is created with `/id` as the partition key path.
- Products are stored as an internal document shape and mapped back into domain entities on read.
- `GetAllAsync` queries all items from the container and materializes them as `Product` instances.

## Aspire Host

The Aspire AppHost lives in `src/AppHost/AppHost.cs` and acts as the orchestration entry point for the distributed application.

- It adds `ASPIRE_ALLOW_UNSECURED_TRANSPORT = true` to configuration for local development.
- It registers the API project so Aspire can start it.

In practice, AppHost is the developer entry point for running the system as a small Aspire application, while the API project remains focused on HTTP behavior.

## Configuration

The API currently reads only a small amount of configuration:

- Logging defaults from `appsettings.json` and `appsettings.Development.json`.
- Cosmos DB settings from the `CosmosDb` configuration section when present.

If the Cosmos settings are missing or incomplete, the application still starts and uses the in-memory repository.

## Design Principles

The current codebase follows a few simple rules:

- Keep `Program.cs` small and focused on composition.
- Put business rules in the domain entity, not in the endpoint.
- Hide persistence behind an interface.
- Prefer a safe fallback for local development.
- Keep Aspire orchestration separate from the API runtime.
