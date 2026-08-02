# Configuration

This document explains how the application is configured at startup and which settings affect runtime behavior.

## Overview

The API reads configuration from the standard ASP.NET Core configuration pipeline. The current setup uses:

- `src/Api/appsettings.json`
- `src/Api/appsettings.Development.json`
- environment variables or other providers supplied by ASP.NET Core

The main configuration decision in the app is whether Cosmos DB is fully configured. If it is, the API uses Cosmos. If not, it falls back to the in-memory repository.

## Where Configuration Is Used

`src/Api/Program.cs` binds the `CosmosDb` section into `CosmosDbOptions` and checks `IsConfigured` before selecting the repository implementation.

- If `IsConfigured` is `true`, the app uses `CosmosProductRepository`.
- If `IsConfigured` is `false`, the app uses `InMemoryProductRepository`.

This makes the API runnable even when external infrastructure is not available.

## Cosmos DB Settings

The Cosmos settings live under the `CosmosDb` section and are defined by `src/Infrastructure/Configuration/CosmosDbOptions.cs`.

Required values:

- `CosmosDb:ConnectionString`
- `CosmosDb:DatabaseName`
- `CosmosDb:ContainerName`

`CosmosDbOptions.IsConfigured` returns `true` only when all three values are present and not blank.

If the section is missing or incomplete, the Cosmos repository is not used.

## Current App Settings

### `src/Api/appsettings.json`

The base API settings currently contain:

- Logging defaults.
- `AllowedHosts` set to `*`.

This file does not currently define a `CosmosDb` section.

### `src/Api/appsettings.Development.json`

The development settings currently override logging:

- `Default` logging level is set to `Debug`.
- `Microsoft.AspNetCore` remains at `Warning`.

This keeps development output more verbose while leaving ASP.NET Core framework logs at a lower level.

## Aspire Configuration

`src/AppHost/AppHost.cs` adds `ASPIRE_ALLOW_UNSECURED_TRANSPORT = true` to configuration for local development.

That setting is specific to the Aspire host and is used so the local application can run without requiring secured transport during development.

## Runtime Behavior

The configuration model currently leads to three practical runtime outcomes:

1. Cosmos DB is fully configured, so the API uses `CosmosProductRepository`.
2. Cosmos DB is not configured, so the API uses `InMemoryProductRepository`.
3. Aspire runs the app locally with unsecured transport enabled for the development host.

## Operational Notes

- Keep secrets out of source-controlled `appsettings` files.
- Prefer environment-specific overrides for local development.
- Update this document whenever new configuration sections are added.
