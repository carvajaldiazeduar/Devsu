FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/ClienteService/Domain/ClienteService.Domain.csproj ClienteService/Domain/
COPY src/ClienteService/Application/ClienteService.Application.csproj ClienteService/Application/
COPY src/ClienteService/Infrastructure/ClienteService.Infrastructure.csproj ClienteService/Infrastructure/
COPY src/ClienteService/API/ClienteService.API.csproj ClienteService/API/

RUN dotnet restore ClienteService/API/ClienteService.API.csproj

COPY src/ClienteService/ ClienteService/
WORKDIR /src/ClienteService/API
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ClienteService.API.dll"]
