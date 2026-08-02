# Domain

This document describes the core domain model of the repository.

## Overview

The domain layer is intentionally small. It currently centers on a single entity, `Product`, which represents the business concept handled by the API.

The domain layer is responsible for enforcing rules that should hold regardless of how data is stored or which endpoint created it.

For a broader system view, see [architecture.md](architecture.md).

## Product

`Product` lives in `src/Domain/Entities/Product.cs` and exposes three properties:

- `Id`
- `Name`
- `Price`

The properties are set through the constructor and have private setters so the entity keeps control over its state.

## Domain Rules

`Product` enforces the following invariants when it is created:

- `Name` must not be null, empty, or whitespace.
- `Price` must not be negative.

If either rule is broken, the constructor throws before the object is created successfully.

## Why the Rules Live Here

These validations are placed in the domain entity so they apply everywhere:

- when the API creates a product,
- when application services instantiate the entity,
- and when future callers use the model directly.

This keeps the business rules close to the concept they protect and avoids duplicating validation across endpoints or repositories.

## Domain Responsibilities

The domain layer does:

- represent the product concept,
- enforce the product invariants,
- and protect the entity from invalid state.

The domain layer does not:

- talk to Cosmos DB,
- know about HTTP,
- or decide which repository implementation to use.

## Relationship to the Rest of the System

- `ProductService` creates `Product` instances and passes them to the repository abstraction.
- `IProductRepository` defines persistence without leaking infrastructure concerns into the domain.
- `CosmosProductRepository` and `InMemoryProductRepository` store and return `Product` instances, but they do not own the rules for creating them.

## Current Domain Scope

The domain model is small today, but this structure makes it easy to add more concepts later without mixing them into the API or infrastructure layers.
