FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/Api/NetAspireServer.Api.csproj", "src/Api/"]
COPY ["src/Application/NetAspireServer.Application.csproj", "src/Application/"]
COPY ["src/Domain/NetAspireServer.Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/NetAspireServer.Infrastructure.csproj", "src/Infrastructure/"]
RUN dotnet restore "src/Api/NetAspireServer.Api.csproj"
COPY . .
WORKDIR "/src/src/Api"
RUN dotnet publish "NetAspireServer.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENTRYPOINT ["dotnet", "NetAspireServer.Api.dll"]
