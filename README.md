# Net Aspire Server

Este proyecto muestra una arquitectura limpia con .NET, Aspire, Cosmos DB y Docker.

## Requisitos

- .NET 10 SDK
- Docker Desktop

## Ejecutar la API localmente

```bash
dotnet build
 dotnet run --project src/Api/NetAspireServer.Api.csproj
```

## Ejecutar con Docker

```bash
docker compose up --build
```

La API quedará disponible en:

- http://localhost:8080/
- http://localhost:8080/health

## Ejemplo de uso

### Crear un producto

```bash
curl -X POST http://localhost:8080/products \
  -H "Content-Type: application/json" \
  -d '{"name":"Laptop","price":999.99}'
```

### Listar productos

```bash
curl http://localhost:8080/products
```
