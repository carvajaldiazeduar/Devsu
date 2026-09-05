FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/CuentaMovimientoService/Domain/CuentaMovimientoService.Domain.csproj CuentaMovimientoService/Domain/
COPY src/CuentaMovimientoService/Application/CuentaMovimientoService.Application.csproj CuentaMovimientoService/Application/
COPY src/CuentaMovimientoService/Infrastructure/CuentaMovimientoService.Infrastructure.csproj CuentaMovimientoService/Infrastructure/
COPY src/CuentaMovimientoService/API/CuentaMovimientoService.API.csproj CuentaMovimientoService/API/

RUN dotnet restore CuentaMovimientoService/API/CuentaMovimientoService.API.csproj

COPY src/CuentaMovimientoService/ CuentaMovimientoService/
WORKDIR /src/CuentaMovimientoService/API
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CuentaMovimientoService.API.dll"]
