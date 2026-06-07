# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["nuget.config", "."]
COPY ["src/MyTelegram.GatewayServer/MyTelegram.GatewayServer.csproj", "src/MyTelegram.GatewayServer/"]
COPY ["src/MyTelegram.Core/MyTelegram.Core.csproj", "src/MyTelegram.Core/"]
COPY ["src/MyTelegram.Abstractions/MyTelegram.Abstractions.csproj", "src/MyTelegram.Abstractions/"]
COPY ["src/MyTelegram.Domain.Shared/MyTelegram.Domain.Shared.csproj", "src/MyTelegram.Domain.Shared/"]
COPY ["src/MyTelegram.Schema/MyTelegram.Schema.csproj", "src/MyTelegram.Schema/"]
COPY ["src/MyTelegram.EventBus.Rebus/MyTelegram.EventBus.Rebus.csproj", "src/MyTelegram.EventBus.Rebus/"]
COPY ["src/MyTelegram.EventBus/MyTelegram.EventBus.csproj", "src/MyTelegram.EventBus/"]
COPY ["src/MyTelegram.MTProto/MyTelegram.MTProto.csproj", "src/MyTelegram.MTProto/"]
RUN --mount=type=cache,id=nuget-mytelegram-gateway-server,target=/root/.nuget/packages \
	dotnet restore "./src/MyTelegram.GatewayServer/MyTelegram.GatewayServer.csproj"
COPY src/MyTelegram.GatewayServer/. ./src/MyTelegram.GatewayServer/
COPY src/MyTelegram.Core/. ./src/MyTelegram.Core/
COPY src/MyTelegram.EventBus.RabbitMQ/. ./src/MyTelegram.EventBus.RabbitMQ/
COPY src/MyTelegram.MTProto/. ./src/MyTelegram.MTProto/
COPY src/MyTelegram.Abstractions/. ./src/MyTelegram.Abstractions/
COPY src/MyTelegram.Domain.Shared/. ./src/MyTelegram.Domain.Shared/
COPY src/MyTelegram.Schema/. ./src/MyTelegram.Schema/
COPY src/MyTelegram.EventBus/. ./src/MyTelegram.EventBus/
COPY src/MyTelegram.EventFlow/. ./src/MyTelegram.EventFlow/
COPY src/MyTelegram.Messenger/. ./src/MyTelegram.Messenger/
COPY src/MyTelegram.ReadModel.Impl/. ./src/MyTelegram.ReadModel.Impl/
COPY src/MyTelegram.ReadModel.Interfaces/. ./src/MyTelegram.ReadModel.Interfaces/
COPY src/MyTelegram.Queries/. ./src/MyTelegram.Queries/
COPY src/MyTelegram.QueryHandlers.MongoDB/. ./src/MyTelegram.QueryHandlers.MongoDB/
COPY src/MyTelegram.MTProto/. ./src/MyTelegram.MTProto/
COPY src/Microsoft.Cuckoo/. ./src/Microsoft.Cuckoo/
WORKDIR "/src/src/MyTelegram.GatewayServer"
RUN --mount=type=cache,id=nuget-mytelegram-gateway-server,target=/root/.nuget/packages \
	--mount=type=cache,id=msbuild-mytelegram-gateway-server,target=/src/.msbuild \
	dotnet build "./MyTelegram.GatewayServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MyTelegram.GatewayServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyTelegram.GatewayServer.dll"]