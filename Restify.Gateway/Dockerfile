# Gateway Service Dockerfile
# Build context: repo root (BackEnd/)
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Copy project file
COPY ["Restify.Gateway/Restify.Gateway.csproj", "Restify.Gateway/"]

# Restore dependencies
RUN dotnet restore "Restify.Gateway/Restify.Gateway.csproj"

# Copy remaining files
COPY Restify.Gateway/ Restify.Gateway/

# Publish
RUN dotnet publish "Restify.Gateway/Restify.Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Restify.Gateway.dll"]
