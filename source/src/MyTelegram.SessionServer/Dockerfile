FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/MyTelegram.SessionServer/MyTelegram.SessionServer.csproj", "src/MyTelegram.SessionServer/"]
RUN dotnet restore "./src/MyTelegram.SessionServer/MyTelegram.SessionServer.csproj"
COPY . .
WORKDIR "/src/src/MyTelegram.SessionServer"
RUN dotnet build "./MyTelegram.SessionServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MyTelegram.SessionServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyTelegram.SessionServer.dll"]
