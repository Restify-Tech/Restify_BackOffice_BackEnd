# Auth Service Dockerfile
# Build context: repo root (BackEnd/)
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Copy project files
COPY ["Restify.Auth/src/Restify.Auth.Api/Restify.Auth.Api.csproj", "Restify.Auth/src/Restify.Auth.Api/"]
COPY ["Restify.Auth/src/Restify.Auth.Domain/Restify.Auth.Domain.csproj", "Restify.Auth/src/Restify.Auth.Domain/"]
COPY ["Restify.Auth/src/Restify.Auth.Application/Restify.Auth.Application.csproj", "Restify.Auth/src/Restify.Auth.Application/"]
COPY ["Restify.Auth/src/Restify.Auth.Infrastructure/Restify.Auth.Infrastructure.csproj", "Restify.Auth/src/Restify.Auth.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Restify.Auth/src/Restify.Auth.Api/Restify.Auth.Api.csproj"

# Copy remaining files
COPY Restify.Auth/ Restify.Auth/

# Publish
RUN dotnet publish "Restify.Auth/src/Restify.Auth.Api/Restify.Auth.Api.csproj" -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Restify.Auth.Api.dll"]
