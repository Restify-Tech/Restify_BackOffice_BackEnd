# BackOffice Service Dockerfile
# Build context: ./BackEnd (para acceder a Restify.Core)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy BackOffice project files
COPY ["Restify.BackOffice/src/Restify.BackOffice.Api/Restify.BackOffice.Api.csproj", "Restify.BackOffice/src/Restify.BackOffice.Api/"]
COPY ["Restify.BackOffice/src/Restify.BackOffice.Application/Restify.BackOffice.Application.csproj", "Restify.BackOffice/src/Restify.BackOffice.Application/"]
COPY ["Restify.BackOffice/src/Restify.BackOffice.Domain/Restify.BackOffice.Domain.csproj", "Restify.BackOffice/src/Restify.BackOffice.Domain/"]
COPY ["Restify.BackOffice/src/Restify.BackOffice.Infrastructure/Restify.BackOffice.Infrastructure.csproj", "Restify.BackOffice/src/Restify.BackOffice.Infrastructure/"]

# Copy Core project files (dependencia)
COPY ["Restify.Core/src/Restify.Core.Application/Restify.Core.Application.csproj", "Restify.Core/src/Restify.Core.Application/"]
COPY ["Restify.Core/src/Restify.Core.Domain/Restify.Core.Domain.csproj", "Restify.Core/src/Restify.Core.Domain/"]
COPY ["Restify.Core/src/Restify.Core.Infrastructure/Restify.Core.Infrastructure.csproj", "Restify.Core/src/Restify.Core.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Restify.BackOffice/src/Restify.BackOffice.Api/Restify.BackOffice.Api.csproj"

# Copy remaining files
COPY Restify.BackOffice/ ./Restify.BackOffice/
COPY Restify.Core/ ./Restify.Core/

# Build
WORKDIR "/src/Restify.BackOffice/src/Restify.BackOffice.Api"
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
