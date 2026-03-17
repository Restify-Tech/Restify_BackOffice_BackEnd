# Core Service Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/Restify.Core.Api/Restify.Core.Api.csproj", "src/Restify.Core.Api/"]
COPY ["src/Restify.Core.Application/Restify.Core.Application.csproj", "src/Restify.Core.Application/"]
COPY ["src/Restify.Core.Domain/Restify.Core.Domain.csproj", "src/Restify.Core.Domain/"]
COPY ["src/Restify.Core.Infrastructure/Restify.Core.Infrastructure.csproj", "src/Restify.Core.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/Restify.Core.Api/Restify.Core.Api.csproj"

# Copy remaining files
COPY . .

# Build
WORKDIR "/src/src/Restify.Core.Api"
RUN dotnet build "Restify.Core.Api.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "Restify.Core.Api.csproj" -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "Restify.Core.Api.dll"]
