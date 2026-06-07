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
COPY ["src/MyTelegram.SmsSender/MyTelegram.SmsSender.csproj", "src/MyTelegram.SmsSender/"]
COPY ["src/MyTelegram.Core/MyTelegram.Core.csproj", "src/MyTelegram.Core/"]
COPY ["src/MyTelegram.Abstractions/MyTelegram.Abstractions.csproj", "src/MyTelegram.Abstractions/"]
COPY ["src/MyTelegram.Domain.Shared/MyTelegram.Domain.Shared.csproj", "src/MyTelegram.Domain.Shared/"]
COPY ["src/MyTelegram.Schema/MyTelegram.Schema.csproj", "src/MyTelegram.Schema/"]
COPY ["src/MyTelegram.EventBus.Rebus/MyTelegram.EventBus.Rebus.csproj", "src/MyTelegram.EventBus.Rebus/"]
COPY ["src/MyTelegram.EventBus/MyTelegram.EventBus.csproj", "src/MyTelegram.EventBus/"]
COPY ["src/MyTelegram.EventBus.RabbitMQ/MyTelegram.EventBus.RabbitMQ.csproj", "src/MyTelegram.EventBus.RabbitMQ/"]
COPY ["src/MyTelegram.Services/MyTelegram.Services.csproj", "src/MyTelegram.Services/"]
COPY ["src/MyTelegram.EventFlow/MyTelegram.EventFlow.csproj", "src/MyTelegram.EventFlow/"]
RUN --mount=type=cache,id=nuget-mytelegram-smssender,target=/root/.nuget/packages \
	dotnet restore "./src/MyTelegram.SmsSender/MyTelegram.SmsSender.csproj"
COPY src/MyTelegram.SmsSender/. ./src/MyTelegram.SmsSender/
COPY src/MyTelegram.Core/. ./src/MyTelegram.Core/
COPY src/MyTelegram.EventBus.RabbitMQ/. ./src/MyTelegram.EventBus.RabbitMQ/
COPY src/MyTelegram.Services/. ./src/MyTelegram.Services/
COPY src/MyTelegram.Abstractions/. ./src/MyTelegram.Abstractions/
COPY src/MyTelegram.Domain.Shared/. ./src/MyTelegram.Domain.Shared/
COPY src/MyTelegram.Schema/. ./src/MyTelegram.Schema/
COPY src/MyTelegram.EventBus/. ./src/MyTelegram.EventBus/
COPY src/MyTelegram.EventFlow/. ./src/MyTelegram.EventFlow/
WORKDIR "/src/src/MyTelegram.SmsSender"
RUN --mount=type=cache,id=nuget-mytelegram-smssender,target=/root/.nuget/packages \
	--mount=type=cache,id=msbuild-mytelegram-smssender,target=/src/.msbuild \
	dotnet build "./MyTelegram.SmsSender.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MyTelegram.SmsSender.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyTelegram.SmsSender.dll"]