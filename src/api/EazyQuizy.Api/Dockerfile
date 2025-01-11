FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5530
EXPOSE 5531

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/api/EazyQuizy.Api/EazyQuizy.Api.csproj", "src/api/EazyQuizy.Api/"]
COPY ["src/core/EazyQuizy.Core.Abstractions/EazyQuizy.Core.Abstractions.csproj", "src/core/EazyQuizy.Core.Abstractions/"]
COPY ["src/common/EazyQuizy.Common.Protos/EazyQuizy.Common.Protos.csproj", "src/common/EazyQuizy.Common.Protos/"]
COPY ["src/common/EazyQuizy.Common/EazyQuizy.Common.csproj", "src/common/EazyQuizy.Common/"]
RUN dotnet restore "src/api/EazyQuizy.Api/EazyQuizy.Api.csproj"
COPY . .
WORKDIR "/src/src/api/EazyQuizy.Api"
RUN dotnet build "EazyQuizy.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EazyQuizy.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EazyQuizy.Api.dll"]
