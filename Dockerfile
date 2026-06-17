# ── Stage 1: Build ────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files for layer caching
COPY ["src/PharmacyBilling.API/PharmacyBilling.API.csproj", "PharmacyBilling.API/"]
COPY ["src/PharmacyBilling.Application/PharmacyBilling.Application.csproj", "PharmacyBilling.Application/"]
COPY ["src/PharmacyBilling.Infrastructure/PharmacyBilling.Infrastructure.csproj", "PharmacyBilling.Infrastructure/"]
COPY ["src/PharmacyBilling.Domain/PharmacyBilling.Domain.csproj", "PharmacyBilling.Domain/"]

# Restore
RUN dotnet restore "PharmacyBilling.API/PharmacyBilling.API.csproj"

# Copy source
COPY src/ .

# Publish
RUN dotnet publish "PharmacyBilling.API/PharmacyBilling.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ── Stage 2: Runtime ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

RUN mkdir -p /app/logs

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "PharmacyBilling.API.dll"]
