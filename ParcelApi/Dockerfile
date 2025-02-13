FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 80

RUN apk add --no-cache iputils curl
HEALTHCHECK CMD curl -f http://localhost:80/health || exit

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
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
RUN apk add --upgrade apk-tools
RUN apk add --no-cache icu-libs
USER $APP_UID
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

ENTRYPOINT ["dotnet", "ParcelApi.dll"]