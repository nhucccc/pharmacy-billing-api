# ── Stage 1: Build ────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files first for layer caching
COPY ["src/PharmacyBilling.API/PharmacyBilling.API.csproj", "PharmacyBilling.API/"]
COPY ["src/PharmacyBilling.Application/PharmacyBilling.Application.csproj", "PharmacyBilling.Application/"]
COPY ["src/PharmacyBilling.Infrastructure/PharmacyBilling.Infrastructure.csproj", "PharmacyBilling.Infrastructure/"]
COPY ["src/PharmacyBilling.Domain/PharmacyBilling.Domain.csproj", "PharmacyBilling.Domain/"]

# Restore
RUN dotnet restore "PharmacyBilling.API/PharmacyBilling.API.csproj"

# Copy source
COPY src/ .

# Build
RUN dotnet build "PharmacyBilling.API/PharmacyBilling.API.csproj" -c Release -o /app/build

# ── Stage 2: Publish ───────────────────────────────────────────────────────────
FROM build AS publish
RUN dotnet publish "PharmacyBilling.API/PharmacyBilling.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ── Stage 3: Runtime ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Create logs directory
RUN mkdir -p /app/logs

# Non-root user for security
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

COPY --from=publish /app/publish .

EXPOSE 8080

ENV ASPNETCORE_ENVIRONMENT=Docker
ENV ASPNETCORE_URLS=http://+:8080

HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "PharmacyBilling.API.dll"]
