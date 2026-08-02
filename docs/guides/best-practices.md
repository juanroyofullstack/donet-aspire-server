# Best Practices

This document captures the practical standards that should guide day-to-day development in this repository.

## Purpose

The goal is to keep the codebase predictable, easy to evolve, and simple to reason about for both humans and LLM-assisted workflows.

## Architecture

- Keep `Program.cs` focused on composition.
- Keep HTTP concerns in the API layer.
- Keep application logic in `Application`.
- Keep business rules in `Domain`.
- Keep persistence and external integrations in `Infrastructure`.

## Async and Cancellation

- Use async methods for I/O-bound operations.
- Propagate `CancellationToken` from the HTTP layer through the service layer and into repositories.
- Avoid blocking calls such as `.Result`, `.Wait()`, or `GetAwaiter().GetResult()`.
- Use `Task` and `Task<T>` for async APIs unless there is a clear performance reason to consider `ValueTask`.

## Validation

- Validate input as early as possible.
- Keep invariant enforcement in the domain model.
- Return predictable API responses for invalid input instead of letting exceptions leak to the client.
- Use explicit exceptions only when the failure is truly exceptional.

## Dependency Injection

- Prefer constructor injection.
- Validate required dependencies with `ArgumentNullException` when a class can be constructed directly.
- Keep service lifetimes aligned with the behavior of the dependency.
- Depend on interfaces for replaceable behavior.

## Repositories

- Keep repositories focused on persistence only.
- Preserve the domain model when reading and writing.
- Keep implementation details out of the application layer.
- Make fallback implementations safe under concurrent access if they are registered as singletons.

## Error Handling

- Return clear error responses for expected failures.
- Keep exception messages specific and actionable.
- Use structured logging when diagnosing failures.
- Handle infrastructure failures close to the boundary where they occur.

## Testing

- Test domain rules directly.
- Test application services against repository abstractions.
- Test API endpoints for transport behavior.
- Cover both success and failure paths.
- Keep tests small and focused on one behavior.

## Configuration

- Use strongly typed configuration objects for settings that affect behavior.
- Keep secrets out of source-controlled settings files.
- Make fallback behavior explicit in documentation and code.
- Validate required configuration before using infrastructure services.

## Documentation

- Keep docs aligned with code.
- Use relative links inside the `docs/` folder.
- Update the reference docs when behavior changes.
- Keep LLM-facing docs compact and explicit.

## Review Checklist

Before merging a change, check that:

- the change stays in the correct layer,
- async flows propagate cancellation,
- validation happens in the right place,
- repository behavior is safe,
- and the docs still match the code.
