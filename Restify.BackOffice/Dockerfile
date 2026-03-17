# BackOffice Service Dockerfile
# Build context: repo root (BackEnd/)
# Note: BackOffice references Restify.Core projects
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Copy BackOffice project files
COPY ["Restify.BackOffice/src/Restify.BackOffice.Api/Restify.BackOffice.Api.csproj", "Restify.BackOffice/src/Restify.BackOffice.Api/"]
COPY ["Restify.BackOffice/src/Restify.BackOffice.Domain/Restify.BackOffice.Domain.csproj", "Restify.BackOffice/src/Restify.BackOffice.Domain/"]
COPY ["Restify.BackOffice/src/Restify.BackOffice.Application/Restify.BackOffice.Application.csproj", "Restify.BackOffice/src/Restify.BackOffice.Application/"]
COPY ["Restify.BackOffice/src/Restify.BackOffice.Infrastructure/Restify.BackOffice.Infrastructure.csproj", "Restify.BackOffice/src/Restify.BackOffice.Infrastructure/"]

# Copy Core project files (cross-project dependency)
COPY ["Restify.Core/src/Restify.Core.Domain/Restify.Core.Domain.csproj", "Restify.Core/src/Restify.Core.Domain/"]
COPY ["Restify.Core/src/Restify.Core.Application/Restify.Core.Application.csproj", "Restify.Core/src/Restify.Core.Application/"]
COPY ["Restify.Core/src/Restify.Core.Infrastructure/Restify.Core.Infrastructure.csproj", "Restify.Core/src/Restify.Core.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Restify.BackOffice/src/Restify.BackOffice.Api/Restify.BackOffice.Api.csproj"

# Copy remaining files
COPY Restify.BackOffice/ Restify.BackOffice/
COPY Restify.Core/ Restify.Core/

# Publish
RUN dotnet publish "Restify.BackOffice/src/Restify.BackOffice.Api/Restify.BackOffice.Api.csproj" -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Restify.BackOffice.Api.dll"]
