# Auth Service Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/Restify.Auth.Api/Restify.Auth.Api.csproj", "src/Restify.Auth.Api/"]
COPY ["src/Restify.Auth.Application/Restify.Auth.Application.csproj", "src/Restify.Auth.Application/"]
COPY ["src/Restify.Auth.Domain/Restify.Auth.Domain.csproj", "src/Restify.Auth.Domain/"]
COPY ["src/Restify.Auth.Infrastructure/Restify.Auth.Infrastructure.csproj", "src/Restify.Auth.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/Restify.Auth.Api/Restify.Auth.Api.csproj"

# Copy remaining files
COPY . .

# Build
WORKDIR "/src/src/Restify.Auth.Api"
RUN dotnet build "Restify.Auth.Api.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "Restify.Auth.Api.csproj" -c Release -o /app/publish

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
ENTRYPOINT ["dotnet", "Restify.Auth.Api.dll"]
