# AGENTS.md

## Purpose

This repository is a .NET Aspire solution with a Minimal API, layered application code, and Markdown documentation that should stay aligned with the code.

## Working Rules

- Prefer small, focused changes.
- Keep `Program.cs` as the composition root only.
- Keep business rules in `Domain`.
- Keep orchestration in `Application`.
- Keep persistence in `Infrastructure`.
- Keep API endpoints thin.

## Documentation Rules

- Treat `docs/` as part of the source of truth for architecture, configuration, API behavior, and LLM guidance.
- Use relative links inside `docs/`.
- Update the relevant docs when behavior changes.
- Keep LLM-facing docs short, explicit, and easy to scan.

## Async Rules

- Use `async` and `await` for I/O-bound work.
- Propagate `CancellationToken` from the API layer through services and repositories.
- Avoid `.Result`, `.Wait()`, and `GetAwaiter().GetResult()`.
- Keep async methods named with the `Async` suffix.

## Validation Rules

- Validate input at the boundary.
- Enforce invariants in the domain model.
- Use explicit exceptions for invalid state inside the model.
- Prefer predictable API responses for expected failures.

## Repository Rules

- Keep repositories behind interfaces.
- Preserve the domain model when reading and writing data.
- Make fallback implementations safe for concurrent access when used as singletons.
- Do not leak storage details into the API or domain layers.

## Review and Verification

- After code changes, validate with the narrowest useful check available.
- Prefer focused tests or targeted error checks for the touched area.
- Do not expand scope to unrelated code while fixing a local issue.

## When Editing This Repo

- Preserve existing structure unless the task explicitly requires a reorganization.
- Keep Markdown portable and repository-relative.
- If a change touches behavior, update the matching docs in `docs/`.
