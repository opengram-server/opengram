# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["nuget.config", "."]
COPY ["src/MyTelegram.DataSeeder/MyTelegram.DataSeeder.csproj", "src/MyTelegram.DataSeeder/"]
COPY ["src/MyTelegram.Domain/MyTelegram.Domain.csproj", "src/MyTelegram.Domain/"]
COPY ["src/MyTelegram.Domain.Shared/MyTelegram.Domain.Shared.csproj", "src/MyTelegram.Domain.Shared/"]
COPY ["src/MyTelegram.Abstractions/MyTelegram.Abstractions.csproj", "src/MyTelegram.Abstractions/"]
COPY ["src/MyTelegram.Schema/MyTelegram.Schema.csproj", "src/MyTelegram.Schema/"]
COPY ["src/MyTelegram.EventFlow/MyTelegram.EventFlow.csproj", "src/MyTelegram.EventFlow/"]
COPY ["src/MyTelegram.QueryHandlers.MongoDB/MyTelegram.QueryHandlers.MongoDB.csproj", "src/MyTelegram.QueryHandlers.MongoDB/"]
COPY ["src/MyTelegram.Queries/MyTelegram.Queries.csproj", "src/MyTelegram.Queries/"]
COPY ["src/MyTelegram.ReadModel/MyTelegram.ReadModel.csproj", "src/MyTelegram.ReadModel/"]
COPY ["src/MyTelegram.ReadModel.Interfaces/MyTelegram.ReadModel.Interfaces.csproj", "src/MyTelegram.ReadModel.Interfaces/"]
COPY ["src/MyTelegram.Queries/MyTelegram.Queries.csproj", "src/MyTelegram.Queries/"]
COPY ["src/MyTelegram.ReadModel.MongoDB/MyTelegram.ReadModel.MongoDB.csproj", "src/MyTelegram.ReadModel.MongoDB/"]
COPY ["src/MyTelegram.ReadModel.MongoDB/MyTelegram.ReadModel.MongoDB.csproj", "src/MyTelegram.ReadModel.MongoDB/"]
COPY ["src/MyTelegram.EventFlow.MongoDB/MyTelegram.EventFlow.MongoDB.csproj", "src/MyTelegram.EventFlow.MongoDB/"]
COPY ["src/MyTelegram.Services/MyTelegram.Services.csproj", "src/MyTelegram.Services/"]
COPY ["src/MyTelegram.Core/MyTelegram.Core.csproj", "src/MyTelegram.Core/"]
COPY ["src/MyTelegram.EventBus/MyTelegram.EventBus.csproj", "src/MyTelegram.EventBus/"]
COPY ["src/MyTelegram.EventBus.RabbitMQ/MyTelegram.EventBus.RabbitMQ.csproj","src/MyTelegram.EventBus.RabbitMQ/"]
RUN --mount=type=cache,id=nuget-mytelegram-data-seeder,target=/root/.nuget/packages \
	dotnet restore "./src/MyTelegram.DataSeeder/MyTelegram.DataSeeder.csproj"
COPY src/MyTelegram.DataSeeder/. ./src/MyTelegram.DataSeeder/
COPY src/MyTelegram.Domain.Shared/. ./src/MyTelegram.Domain.Shared/
COPY src/MyTelegram.Domain/. ./src/MyTelegram.Domain/
COPY src/MyTelegram.QueryHandlers.MongoDB/. ./src/MyTelegram.QueryHandlers.MongoDB/
COPY src/MyTelegram.ReadModel.Interfaces/. ./src/MyTelegram.ReadModel.Interfaces/
COPY src/MyTelegram.ReadModel/. ./src/MyTelegram.ReadModel/
COPY src/MyTelegram.ReadModel.MongoDB/. ./src/MyTelegram.ReadModel.MongoDB/
COPY src/MyTelegram.Queries/. ./src/MyTelegram.Queries/
COPY src/MyTelegram.Services/. ./src/MyTelegram.Services/
COPY src/MyTelegram.Core/. ./src/MyTelegram.Core/
COPY src/MyTelegram.Abstractions/. ./src/MyTelegram.Abstractions/
COPY src/MyTelegram.Schema/. ./src/MyTelegram.Schema/
COPY src/MyTelegram.EventBus/. ./src/MyTelegram.EventBus/
COPY src/MyTelegram.EventFlow/. ./src/MyTelegram.EventFlow/
COPY src/MyTelegram.EventFlow.MongoDB/. ./src/MyTelegram.EventFlow.MongoDB/
COPY src/MyTelegram.EventBus.RabbitMQ/. ./src/MyTelegram.EventBus.RabbitMQ/
COPY src/MyTelegram.MTProto/. ./src/MyTelegram.MTProto/
COPY src/MyTelegram.Messenger/. ./src/MyTelegram.Messenger/
COPY src/Microsoft.Cuckoo/. ./src/Microsoft.Cuckoo/
COPY src/MyTelegram.Caching.Redis/. ./src/MyTelegram.Caching.Redis/
WORKDIR "/src/src/MyTelegram.DataSeeder"
RUN --mount=type=cache,id=nuget-mytelegram-data-seeder,target=/root/.nuget/packages \
	--mount=type=cache,id=msbuild-mytelegram-data-seeder,target=/src/.msbuild \
	dotnet build "./MyTelegram.DataSeeder.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MyTelegram.DataSeeder.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyTelegram.DataSeeder.dll"]