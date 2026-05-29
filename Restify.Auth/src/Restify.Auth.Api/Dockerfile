# Auth Service Dockerfile
# Build context: Taku_Soft_Tech/ root (includes Restify/ and Services/Observability/)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy Observability package project
COPY ["Services/Observability/package/TakuSoft.Observability/TakuSoft.Observability.csproj", "Services/Observability/package/TakuSoft.Observability/"]

# Copy Auth project files
COPY ["Restify/BackEnd/Restify.Auth/src/Restify.Auth.Api/Restify.Auth.Api.csproj", "Restify/BackEnd/Restify.Auth/src/Restify.Auth.Api/"]
COPY ["Restify/BackEnd/Restify.Auth/src/Restify.Auth.Application/Restify.Auth.Application.csproj", "Restify/BackEnd/Restify.Auth/src/Restify.Auth.Application/"]
COPY ["Restify/BackEnd/Restify.Auth/src/Restify.Auth.Domain/Restify.Auth.Domain.csproj", "Restify/BackEnd/Restify.Auth/src/Restify.Auth.Domain/"]
COPY ["Restify/BackEnd/Restify.Auth/src/Restify.Auth.Infrastructure/Restify.Auth.Infrastructure.csproj", "Restify/BackEnd/Restify.Auth/src/Restify.Auth.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Restify/BackEnd/Restify.Auth/src/Restify.Auth.Api/Restify.Auth.Api.csproj"

# Copy source files
COPY Services/Observability/package/TakuSoft.Observability/ Services/Observability/package/TakuSoft.Observability/
COPY Restify/BackEnd/Restify.Auth/src/ Restify/BackEnd/Restify.Auth/src/

# Build
WORKDIR "/src/Restify/BackEnd/Restify.Auth/src/Restify.Auth.Api"
RUN dotnet build "Restify.Auth.Api.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "Restify.Auth.Api.csproj" -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Restify.Auth.Api.dll"]
