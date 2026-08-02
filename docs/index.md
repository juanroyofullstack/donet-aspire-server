# Documentation Index

This folder contains the living documentation for the repository. The goal is to make the project easy to understand for both humans and LLMs without reading the whole codebase first.

## Folder Layout

- `reference/`: architecture, API, domain, and configuration reference material.
- `guides/`: architecture and LLM working guidelines.
- `context/`: compact LLM-oriented summary.

## Start Here

- [Architecture](reference/architecture.md): explains the layered structure, request flow, repository selection, and Aspire host behavior.
- [LLM Context](context/llm-context.md): compact summary optimized for fast consumption by language models.

## Reference Guides

- [API](reference/api.md): endpoint reference, request and response shapes, and examples.
- [Domain](reference/domain.md): entity rules, invariants, and domain concepts.
- [Configuration](reference/configuration.md): application settings and environment-specific behavior.
- [Architecture Guidelines](guides/architecture-guidelines.md): rules for structuring code and keeping the architecture clean.
- [Best Practices](guides/best-practices.md): practical guidance for writing and reviewing code in this repository.
- [LLM Guidelines](guides/llm-guidelines.md): rules for using LLM assistance effectively in this repository.

## Source of Truth

The documentation is based on the current code under `src/` and should stay aligned with implementation changes.

- `Program.cs` is the composition root for the API.
- `ProductService` coordinates product use cases.
- `Product` owns the core validation rules.
- `CosmosProductRepository` and `InMemoryProductRepository` are the two persistence paths.
- `AppHost.cs` is the Aspire orchestration entry point.

## Maintenance Notes

- Keep links relative so the docs remain portable.
- Prefer short, explicit sections over long prose.
- Update the docs when the API surface, domain rules, or repository behavior changes.
