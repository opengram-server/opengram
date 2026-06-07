FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/MyTelegram.Admin.Api/MyTelegram.Admin.Api.csproj", "src/MyTelegram.Admin.Api/"]
COPY ["src/MyTelegram.Domain/MyTelegram.Domain.csproj", "src/MyTelegram.Domain/"]
COPY ["src/MyTelegram.Domain.Shared/MyTelegram.Domain.Shared.csproj", "src/MyTelegram.Domain.Shared/"]
COPY ["src/MyTelegram.Services/MyTelegram.Services.csproj", "src/MyTelegram.Services/"]
COPY ["src/MyTelegram.Queries/MyTelegram.Queries.csproj", "src/MyTelegram.Queries/"]
COPY ["src/MyTelegram.ReadModel.Interfaces/MyTelegram.ReadModel.Interfaces.csproj", "src/MyTelegram.ReadModel.Interfaces/"]
COPY ["src/MyTelegram.ReadModel/MyTelegram.ReadModel.csproj", "src/MyTelegram.ReadModel/"]
COPY ["src/MyTelegram.Schema/MyTelegram.Schema.csproj", "src/MyTelegram.Schema/"]
COPY ["src/MyTelegram.Core/MyTelegram.Core.csproj", "src/MyTelegram.Core/"]
COPY ["src/MyTelegram.EventFlow/MyTelegram.EventFlow.csproj", "src/MyTelegram.EventFlow/"]
COPY ["src/MyTelegram.EventFlow.MongoDB/MyTelegram.EventFlow.MongoDB.csproj", "src/MyTelegram.EventFlow.MongoDB/"]
COPY ["src/MyTelegram.EventBus/MyTelegram.EventBus.csproj", "src/MyTelegram.EventBus/"]
COPY ["src/MyTelegram.EventBus.RabbitMQ/MyTelegram.EventBus.RabbitMQ.csproj", "src/MyTelegram.EventBus.RabbitMQ/"]
COPY ["src/MyTelegram.Abstractions/MyTelegram.Abstractions.csproj", "src/MyTelegram.Abstractions/"]

COPY ["src/Microsoft.Cuckoo/Microsoft.Cuckoo.csproj", "src/Microsoft.Cuckoo/"]
COPY ["src/MyTelegram.Caching.Redis/MyTelegram.Caching.Redis.csproj", "src/MyTelegram.Caching.Redis/"]
COPY ["src/MyTelegram.GrpcService/MyTelegram.GrpcService.csproj", "src/MyTelegram.GrpcService/"]
COPY ["src/MyTelegram.Converters/MyTelegram.Converters.csproj", "src/MyTelegram.Converters/"]
COPY ["src/MyTelegram.QueryHandlers.MongoDB/MyTelegram.QueryHandlers.MongoDB.csproj", "src/MyTelegram.QueryHandlers.MongoDB/"]
COPY ["src/MyTelegram.QueryHandlers.InMemory/MyTelegram.QueryHandlers.InMemory.csproj", "src/MyTelegram.QueryHandlers.InMemory/"]
COPY ["src/MyTelegram.ReadModel.MongoDB/MyTelegram.ReadModel.MongoDB.csproj", "src/MyTelegram.ReadModel.MongoDB/"]
COPY ["src/MyTelegram.ReadModel.InMemory/MyTelegram.ReadModel.InMemory.csproj", "src/MyTelegram.ReadModel.InMemory/"]
COPY ["src/MyTelegram.MTProto/MyTelegram.MTProto.csproj", "src/MyTelegram.MTProto/"]
COPY ["src/MyTelegram.BotApi/MyTelegram.BotApi.csproj", "src/MyTelegram.BotApi/"]

COPY src/MyTelegram.ReadModel.MongoDB/. ./src/MyTelegram.ReadModel.MongoDB/
COPY src/MyTelegram.QueryHandlers.MongoDB/. ./src/MyTelegram.QueryHandlers.MongoDB/
COPY src/MyTelegram.ReadModel/. ./src/MyTelegram.ReadModel/
COPY src/MyTelegram.ReadModel.Impl/. ./src/MyTelegram.ReadModel.Impl/
COPY src/MyTelegram.EventFlow/. ./src/MyTelegram.EventFlow/
COPY src/MyTelegram.EventFlow.MongoDB/. ./src/MyTelegram.EventFlow.MongoDB/

# Ensure Converters and Caching sources are present before restore/build
COPY src/MyTelegram.Converters/. ./src/MyTelegram.Converters/
COPY src/MyTelegram.Caching.Redis/. ./src/MyTelegram.Caching.Redis/

# Full module sources will be copied after restore for build

RUN --mount=type=cache,id=nuget-mytelegram-admin-api,target=/root/.nuget/packages \
	dotnet restore "./src/MyTelegram.Admin.Api/MyTelegram.Admin.Api.csproj"
COPY src/MyTelegram.Admin.Api/. ./src/MyTelegram.Admin.Api/
COPY src/MyTelegram.Domain/. ./src/MyTelegram.Domain/
COPY src/MyTelegram.Domain.Shared/. ./src/MyTelegram.Domain.Shared/
COPY src/MyTelegram.Services/. ./src/MyTelegram.Services/
COPY src/MyTelegram.Queries/. ./src/MyTelegram.Queries/
COPY src/MyTelegram.GrpcService/. ./src/MyTelegram.GrpcService/
COPY src/MyTelegram.EventFlow/. ./src/MyTelegram.EventFlow/
COPY src/MyTelegram.EventFlow.MongoDB/. ./src/MyTelegram.EventFlow.MongoDB/
COPY src/MyTelegram.EventBus.RabbitMQ/. ./src/MyTelegram.EventBus.RabbitMQ/
COPY src/MyTelegram.Messenger/. ./src/MyTelegram.Messenger/
COPY src/MyTelegram.Core/. ./src/MyTelegram.Core/
COPY src/MyTelegram.Abstractions/. ./src/MyTelegram.Abstractions/
COPY src/MyTelegram.Schema/. ./src/MyTelegram.Schema/
COPY src/MyTelegram.EventBus/. ./src/MyTelegram.EventBus/
COPY src/MyTelegram.ReadModel.Impl/. ./src/MyTelegram.ReadModel/Impl/
COPY src/MyTelegram.ReadModel.Interfaces/. ./src/MyTelegram.ReadModel.Interfaces/
COPY src/MyTelegram.Queries/. ./src/MyTelegram.Queries/
COPY src/MyTelegram.QueryHandlers.MongoDB/. ./src/MyTelegram.QueryHandlers.MongoDB/
COPY src/MyTelegram.MTProto/. ./src/MyTelegram.MTProto/
COPY src/Microsoft.Cuckoo/. ./src/Microsoft.Cuckoo/
WORKDIR "/src/src/MyTelegram.Admin.Api"
RUN --mount=type=cache,id=nuget-mytelegram-admin-api,target=/root/.nuget/packages \
	--mount=type=cache,id=msbuild-mytelegram-admin-api,target=/src/.msbuild \
	dotnet build "./MyTelegram.Admin.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN --mount=type=cache,id=nuget-mytelegram-admin-api,target=/root/.nuget/packages \
	--mount=type=cache,id=msbuild-mytelegram-admin-api,target=/src/.msbuild \
	dotnet publish "./MyTelegram.Admin.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyTelegram.Admin.Api.dll"]
