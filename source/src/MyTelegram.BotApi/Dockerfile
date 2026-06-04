FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy MSBuild props files for centralized package management
COPY ["Directory.Packages.props", "./"]
COPY ["Directory.Build.props", "./"]
COPY ["common.props", "./"]

# Copy project files for BotApi and all dependencies
COPY ["src/MyTelegram.BotApi/MyTelegram.BotApi.csproj", "src/MyTelegram.BotApi/"]
COPY ["src/MyTelegram.Domain.Shared/MyTelegram.Domain.Shared.csproj", "src/MyTelegram.Domain.Shared/"]
COPY ["src/MyTelegram.ReadModel.Interfaces/MyTelegram.ReadModel.Interfaces.csproj", "src/MyTelegram.ReadModel.Interfaces/"]
COPY ["src/MyTelegram.ReadModel/MyTelegram.ReadModel.csproj", "src/MyTelegram.ReadModel/"]
COPY ["src/MyTelegram.ReadModel.MongoDB/MyTelegram.ReadModel.MongoDB.csproj", "src/MyTelegram.ReadModel.MongoDB/"]
COPY ["src/MyTelegram.EventBus.RabbitMQ/MyTelegram.EventBus.RabbitMQ.csproj", "src/MyTelegram.EventBus.RabbitMQ/"]
COPY ["src/MyTelegram.Abstractions/MyTelegram.Abstractions.csproj", "src/MyTelegram.Abstractions/"]
COPY ["src/MyTelegram.Domain/MyTelegram.Domain.csproj", "src/MyTelegram.Domain/"]
COPY ["src/MyTelegram.EventFlow.MongoDB/MyTelegram.EventFlow.MongoDB.csproj", "src/MyTelegram.EventFlow.MongoDB/"]
COPY ["src/MyTelegram.EventFlow/MyTelegram.EventFlow.csproj", "src/MyTelegram.EventFlow/"]
COPY ["src/MyTelegram.EventBus/MyTelegram.EventBus.csproj", "src/MyTelegram.EventBus/"]
COPY ["src/MyTelegram.Queries/MyTelegram.Queries.csproj", "src/MyTelegram.Queries/"]
COPY ["src/MyTelegram.QueryHandlers.MongoDB/MyTelegram.QueryHandlers.MongoDB.csproj", "src/MyTelegram.QueryHandlers.MongoDB/"]
COPY ["src/MyTelegram.Schema/MyTelegram.Schema.csproj", "src/MyTelegram.Schema/"]

# Restore dependencies
RUN dotnet restore "src/MyTelegram.BotApi/MyTelegram.BotApi.csproj"

# Copy all source files
COPY . .

# Build
WORKDIR "/src/src/MyTelegram.BotApi"
RUN dotnet build "MyTelegram.BotApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyTelegram.BotApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyTelegram.BotApi.dll"]
