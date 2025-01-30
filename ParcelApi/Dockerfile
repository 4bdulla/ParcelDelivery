FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80

# healthcheck
RUN apk add curl
HEALTHCHECK CMD curl -f http://localhost:80/health || exit

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ParcelApi/ParcelApi.csproj", "ParcelApi/"]
RUN dotnet restore "ParcelApi/ParcelApi.csproj"
COPY . .
WORKDIR "/src/ParcelApi"
RUN dotnet build "ParcelApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ParcelApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ParcelApi.dll"]
