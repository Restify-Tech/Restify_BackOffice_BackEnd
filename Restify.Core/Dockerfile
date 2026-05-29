# Core Service Dockerfile
# Build context: repo root (BackEnd/)
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Copy project files
COPY ["Restify.Core/src/Restify.Core.Api/Restify.Core.Api.csproj", "Restify.Core/src/Restify.Core.Api/"]
COPY ["Restify.Core/src/Restify.Core.Domain/Restify.Core.Domain.csproj", "Restify.Core/src/Restify.Core.Domain/"]
COPY ["Restify.Core/src/Restify.Core.Application/Restify.Core.Application.csproj", "Restify.Core/src/Restify.Core.Application/"]
COPY ["Restify.Core/src/Restify.Core.Infrastructure/Restify.Core.Infrastructure.csproj", "Restify.Core/src/Restify.Core.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Restify.Core/src/Restify.Core.Api/Restify.Core.Api.csproj"

# Copy remaining files
COPY Restify.Core/ Restify.Core/

# Publish
RUN dotnet publish "Restify.Core/src/Restify.Core.Api/Restify.Core.Api.csproj" -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Restify.Core.Api.dll"]
