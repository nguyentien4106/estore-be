# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/EStore.Api/EStore.Api.csproj", "src/Services/EStore.Api/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Auth/BuildingBlocks.Auth/BuildingBlocks.Auth.csproj", "src/BuildingBlocks/BuildingBlocks.Auth/BuildingBlocks.Auth/"]
COPY ["src/BuildingBlocks/BuildingBlocks/BuildingBlocks.csproj", "src/BuildingBlocks/BuildingBlocks/"]
COPY ["src/Services/EStore.Application/EStore.Application.csproj", "src/Services/EStore.Application/"]
COPY ["src/Services/EStore.Domain/EStore.Domain.csproj", "src/Services/EStore.Domain/"]
COPY ["src/Services/EStore.Infrastructure/EStore.Infrastructure.csproj", "src/Services/EStore.Infrastructure/"]
RUN dotnet restore "./src/Services/EStore.Api/EStore.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/EStore.Api"
RUN dotnet build "./EStore.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EStore.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EStore.Api.dll"]