# LLM Guidelines

This document describes how to use LLM assistance effectively in this repository.

## Goal

The LLM should help you move faster without changing the project architecture by accident.

Use it to:

- explain code,
- draft documentation,
- propose small refactors,
- and clarify behavior before making changes.

Do not use it as a source of truth when the codebase already defines the behavior.

## Read This First

When working in this repo, the best context order is:

1. [docs/index.md](../index.md)
2. [docs/reference/architecture.md](../reference/architecture.md)
3. [docs/reference/configuration.md](../reference/configuration.md)
4. [docs/reference/api.md](../reference/api.md)
5. [docs/reference/domain.md](../reference/domain.md)
6. [docs/context/llm-context.md](../context/llm-context.md)

That order keeps the model anchored in the system shape before it looks at details.

## Source of Truth Hierarchy

Prefer these sources in order:

1. The code under `src/`.
2. The architecture and configuration docs.
3. The API, domain, and context docs.
4. Any prompt or task text from the user.

If there is a conflict, the code wins.

## How to Ask the LLM for Help

Be specific about:

- the file or feature you want to change,
- the behavior you expect,
- the scope you do not want touched,
- and any constraints on style or architecture.

Better prompts mention the exact endpoint, service, or entity involved.

## Good LLM Workflows

- Ask for an explanation before requesting a refactor.
- Ask for a plan before changing multiple layers.
- Keep edits small and reviewable.
- Validate changes against the existing docs and code.

## Things the LLM Should Not Do

- Invent new layers without a clear need.
- Move logic out of the domain without a reason.
- Change repository selection rules casually.
- Rewrite `Program.cs` into a large procedural file.
- Add documentation that contradicts the code.

## Writing Style for LLM Outputs

When asking the LLM to generate content for this repo, prefer outputs that are:

- concise,
- explicit,
- aligned to the current architecture,
- and easy to map back to code files.

Avoid vague guidance like “make it cleaner” unless you also define the target layer or file.

## Review Checklist

Before accepting an LLM-generated change, check that:

- the behavior matches the code,
- the change stays within the correct layer,
- the docs remain accurate,
- and no unrelated files were modified.

## Recommended Output Format

For code changes, ask the LLM to summarize:

- what changed,
- which files changed,
- why the change belongs in that layer,
- and how to validate it.

For documentation changes, ask the LLM to keep links relative and use the current file structure as the reference point.
