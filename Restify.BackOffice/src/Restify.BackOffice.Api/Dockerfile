# BackOffice Service Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/Restify.BackOffice.Api/Restify.BackOffice.Api.csproj", "src/Restify.BackOffice.Api/"]
COPY ["src/Restify.BackOffice.Application/Restify.BackOffice.Application.csproj", "src/Restify.BackOffice.Application/"]
COPY ["src/Restify.BackOffice.Domain/Restify.BackOffice.Domain.csproj", "src/Restify.BackOffice.Domain/"]
COPY ["src/Restify.BackOffice.Infrastructure/Restify.BackOffice.Infrastructure.csproj", "src/Restify.BackOffice.Infrastructure/"]
COPY ["../Restify.Core/src/Restify.Core.Application/Restify.Core.Application.csproj", "src/Restify.Core.Application/"]
COPY ["../Restify.Core/src/Restify.Core.Domain/Restify.Core.Domain.csproj", "src/Restify.Core.Domain/"]
COPY ["../Restify.Core/src/Restify.Core.Infrastructure/Restify.Core.Infrastructure.csproj", "src/Restify.Core.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/Restify.BackOffice.Api/Restify.BackOffice.Api.csproj"

# Copy remaining files
COPY . .

# Build
WORKDIR "/src/src/Restify.BackOffice.Api"
RUN dotnet build "Restify.BackOffice.Api.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "Restify.BackOffice.Api.csproj" -c Release -o /app/publish

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
ENTRYPOINT ["dotnet", "Restify.BackOffice.Api.dll"]
