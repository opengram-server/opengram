FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["nuget.config", "."]
COPY ["src/MyTelegram.SessionServer/MyTelegram.SessionServer.csproj", "src/MyTelegram.SessionServer/"]
COPY ["src/MyTelegram.Core/MyTelegram.Core.csproj", "src/MyTelegram.Core/"]
COPY ["src/MyTelegram.Services/MyTelegram.Services.csproj", "src/MyTelegram.Services/"]
COPY ["src/MyTelegram.EventBus.RabbitMQ/MyTelegram.EventBus.RabbitMQ.csproj", "src/MyTelegram.EventBus.RabbitMQ/"]
COPY ["src/MyTelegram.Caching.Redis/MyTelegram.Caching.Redis.csproj", "src/MyTelegram.Caching.Redis/"]
RUN --mount=type=cache,id=nuget-mytelegram-session-server,target=/root/.nuget/packages \
	dotnet restore "./src/MyTelegram.SessionServer/MyTelegram.SessionServer.csproj"
COPY src/MyTelegram.SessionServer/. ./src/MyTelegram.SessionServer/
COPY src/MyTelegram.Core/. ./src/MyTelegram.Core/
COPY src/MyTelegram.Services/. ./src/MyTelegram.Services/
COPY src/MyTelegram.EventBus.RabbitMQ/. ./src/MyTelegram.EventBus.RabbitMQ/
COPY src/MyTelegram.Caching.Redis/. ./src/MyTelegram.Caching.Redis/
COPY src/MyTelegram.Abstractions/. ./src/MyTelegram.Abstractions/
COPY src/MyTelegram.Domain.Shared/. ./src/MyTelegram.Domain.Shared/
COPY src/MyTelegram.Schema/. ./src/MyTelegram.Schema/
COPY src/MyTelegram.EventBus/. ./src/MyTelegram.EventBus/
COPY src/MyTelegram.EventFlow/. ./src/MyTelegram.EventFlow/
COPY src/MyTelegram.Messenger/. ./src/MyTelegram.Messenger/
WORKDIR "/src/src/MyTelegram.SessionServer"
RUN --mount=type=cache,id=nuget-mytelegram-session-server,target=/root/.nuget/packages \
	--mount=type=cache,id=msbuild-mytelegram-session-server,target=/src/.msbuild \
	dotnet build "./MyTelegram.SessionServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MyTelegram.SessionServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyTelegram.SessionServer.dll"]
