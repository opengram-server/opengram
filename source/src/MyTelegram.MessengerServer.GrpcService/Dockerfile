# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/MyTelegram.MessengerServer.GrpcService/MyTelegram.MessengerServer.GrpcService.csproj", "src/MyTelegram.MessengerServer.GrpcService/"]
COPY ["src/MyTelegram.Queries.MongoDB/MyTelegram.QueryHandlers.MongoDB.csproj", "src/MyTelegram.Queries.MongoDB/"]
COPY ["src/MyTelegram.Queries/MyTelegram.Queries.csproj", "src/MyTelegram.Queries/"]
COPY ["src/MyTelegram.ReadModel/MyTelegram.ReadModel.csproj", "src/MyTelegram.ReadModel/"]
COPY ["src/MyTelegram.Domain/MyTelegram.Domain.csproj", "src/MyTelegram.Domain/"]
COPY ["src/MyTelegram.Domain.Shared/MyTelegram.Domain.Shared.csproj", "src/MyTelegram.Domain.Shared/"]
COPY ["src/MyTelegram.EventFlow/MyTelegram.EventFlow.csproj", "src/MyTelegram.EventFlow/"]
COPY ["src/MyTelegram.Schema/MyTelegram.Schema.csproj", "src/MyTelegram.Schema/"]
COPY ["src/MyTelegram.ReadModel.MongoDB/MyTelegram.ReadModel.MongoDB.csproj", "src/MyTelegram.ReadModel.MongoDB/"]
COPY ["src/MyTelegram.EventFlow.MongoDB/MyTelegram.EventFlow.MongoDB.csproj", "src/MyTelegram.EventFlow.MongoDB/"]
RUN dotnet restore "./src/MyTelegram.MessengerServer.GrpcService/MyTelegram.MessengerServer.GrpcService.csproj"
COPY . .
WORKDIR "/src/src/MyTelegram.MessengerServer.GrpcService"
RUN dotnet build "./MyTelegram.MessengerServer.GrpcService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MyTelegram.MessengerServer.GrpcService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyTelegram.MessengerServer.GrpcService.dll"]