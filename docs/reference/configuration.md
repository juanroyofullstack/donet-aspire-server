# Configuration

This document explains how the application is configured at startup and which settings affect runtime behavior.

## Overview

The API reads configuration from the standard ASP.NET Core configuration pipeline. The current setup uses:

- `src/Api/appsettings.json`
- `src/Api/appsettings.Development.json`
- environment variables or other providers supplied by ASP.NET Core
- connection information injected by Aspire when the API is started through the AppHost

The main configuration decision in the app is whether Cosmos DB is usable. If it is, the API uses Cosmos. If not, it falls back to the in-memory repository.

## Where Configuration Is Used

`src/Api/Program.cs` binds the `CosmosDb` section into `CosmosDbOptions`, conditionally registers an Aspire-managed `CosmosClient`, and checks whether both pieces are available before selecting the repository implementation.

- If `CosmosDbOptions.IsConfigured` is `true` and the `cosmos` connection is present, the app uses `CosmosProductRepository`.
- If either the options are incomplete or the connection is missing, the app uses `InMemoryProductRepository`.

This makes the API runnable even when external infrastructure is not available.

## Cosmos DB Settings

The remaining Cosmos settings live under the `CosmosDb` section and are defined by `src/Infrastructure/Configuration/CosmosDbOptions.cs`.

Required values:

- `CosmosDb:DatabaseName`
- `CosmosDb:ContainerName`

`CosmosDbOptions.IsConfigured` returns `true` only when `DatabaseName` and `ContainerName` are present and not blank.

The connection string is no longer read from the `CosmosDb` section. When the API runs through Aspire, AppHost provides the `cosmos` connection automatically via `WithReference`.

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

If you want the API to use Cosmos while running under Aspire, add a `CosmosDb` section with values that match the AppHost resource names:

- `CosmosDb:DatabaseName = productsdb`
- `CosmosDb:ContainerName = products`

## Aspire Configuration

`src/AppHost/AppHost.cs` adds `ASPIRE_ALLOW_UNSECURED_TRANSPORT = true` to configuration for local development.

It also declares the local Cosmos topology used during Aspire runs:

- a Cosmos DB resource named `cosmos`
- a local emulator via `RunAsEmulator()`
- a database resource named `productsdb`
- a container resource named `products` with `/id` as the partition key path
- a `WithReference(cosmos)` link from AppHost to the API project

The `WithReference(cosmos)` link is what makes the `cosmos` connection string available to the API.

## Runtime Behavior

The configuration model currently leads to three practical runtime outcomes:

1. The API runs under Aspire, the `cosmos` connection is present, and `CosmosDb` contains database and container names, so the API uses `CosmosProductRepository`.
2. The API runs without Aspire or without the `CosmosDb` names, so it falls back to `InMemoryProductRepository`.
3. Aspire runs the app locally with unsecured transport enabled and starts the Cosmos emulator alongside the API.

## Operational Notes

- Keep secrets out of source-controlled `appsettings` files.
- Keep database and container naming consistent between AppHost resources and the `CosmosDb` section.
- Prefer environment-specific overrides for local development.
- Update this document whenever new configuration sections are added.
