# Net Aspire Server

This project demonstrates a clean architecture setup using .NET, Aspire, Cosmos DB, and Docker.

## Requirements

- .NET 10 SDK
- Docker Desktop

## Run the API locally

```bash
dotnet build
dotnet run --project src/Api/NetAspireServer.Api.csproj
```

## Run with Docker

```bash
docker compose up --build
```

The API will be available at:

- http://localhost:8080/
- http://localhost:8080/health

## Run the Cosmos emulator locally

```bash
docker compose up -d cosmos
```

This starts the local Azure Cosmos emulator used by the integration tests. The integration suite connects to https://localhost:8081 and will skip automatically when the emulator is not running.

## Run the test suites

```bash
dotnet test tests/NetAspireServer.Application.Tests.csproj
dotnet test tests/NetAspireServer.IntegrationTests.csproj
```

## Example usage

### Create a product

```bash
curl -X POST http://localhost:8080/products \
  -H "Content-Type: application/json" \
  -d '{"name":"Laptop","price":999.99}'
```

### List products

```bash
curl http://localhost:8080/products
```
