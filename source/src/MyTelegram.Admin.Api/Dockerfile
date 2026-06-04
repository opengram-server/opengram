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

RUN dotnet restore "./src/MyTelegram.Admin.Api/MyTelegram.Admin.Api.csproj"
COPY . .
WORKDIR "/src/src/MyTelegram.Admin.Api"
RUN dotnet build "./MyTelegram.Admin.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MyTelegram.Admin.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyTelegram.Admin.Api.dll"]
