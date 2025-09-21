# ---- runtime base
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://0.0.0.0:5100;http://0.0.0.0:5101 \
    DOTNET_RUNNING_IN_CONTAINER=true
EXPOSE 5100
EXPOSE 5101

# ---- build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY *.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet build "SpotifyMusic.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ---- publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SpotifyMusic.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ---- final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SpotifyMusic.dll"]
