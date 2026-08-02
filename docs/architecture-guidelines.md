# Architecture Guidelines

This document defines the architectural rules for evolving the repository without losing the current structure.

## Core Principles

- Keep the API project thin.
- Keep business rules in the domain model.
- Keep persistence behind interfaces.
- Keep infrastructure isolated from use-case logic.
- Prefer explicit, small dependencies over hidden coupling.

## Layer Responsibilities

### Api

- Compose the application at startup.
- Register dependencies.
- Expose HTTP endpoints.
- Map request data to application calls.

### Application

- Orchestrate use cases.
- Coordinate domain entities and repositories.
- Remain independent from HTTP and storage technologies.

### Domain

- Own the business concepts and invariants.
- Reject invalid state as early as possible.
- Stay free of infrastructure and transport concerns.

### Infrastructure

- Implement repository contracts.
- Handle external systems such as Cosmos DB.
- Provide configuration-backed behavior.

## Dependency Direction

Use dependencies in one direction only:

1. `Api` may depend on `Application` and `Infrastructure`.
2. `Application` may depend on `Domain`.
3. `Infrastructure` may depend on `Application` and `Domain`.
4. `Domain` should not depend on higher layers.

If a change requires reversing that flow, the design should be reconsidered before the code is written.

## Composition Root Rules

`src/Api/Program.cs` should remain the composition root.

Do:

- register services,
- choose implementations,
- map endpoints,
- and wire configuration into the container.

Do not:

- put business rules in `Program.cs`,
- add persistence logic there,
- or grow it into a catch-all application layer.

## Endpoint Rules

- Keep endpoints thin and focused on transport concerns.
- Use request DTOs for input models.
- Delegate use cases to application services.
- Return responses directly from the endpoint without embedding business rules there.
- Keep endpoint logic easy to scan in one screen when possible.

## Service Rules

- Put workflow logic in application services.
- Keep services focused on one use case family.
- Accept cancellation tokens on async methods.
- Avoid direct dependency on concrete repositories.
- Return domain entities or read models, not transport-specific types.

## Repository Rules

- Define persistence through interfaces first.
- Keep repository implementations swappable.
- Preserve the domain model when reading and writing data.
- Use in-memory fallback only as a development-friendly alternative, not as a hidden production mode.

## Configuration Rules

- Treat configuration as input to composition, not business logic.
- Keep connection strings and environment-specific values out of code.
- Use `IsConfigured`-style checks when the app has a safe fallback.
- Document any new configuration section in `docs/configuration.md`.

## Testing Rules

- Test domain rules directly.
- Test application services against repository abstractions.
- Test infrastructure separately when storage behavior matters.
- Keep transport tests focused on request/response behavior.

## Anti-Patterns

- Putting persistence code in endpoints.
- Letting infrastructure types leak into the domain model.
- Duplicating validation in multiple layers.
- Making `Program.cs` responsible for business decisions.
- Adding unrelated features without updating the docs.

## Feature Checklist

Before adding a new feature, check that:

- the domain rule is clear,
- the application service boundary is obvious,
- the repository contract is stable,
- the endpoint stays thin,
- and the docs stay in sync.
