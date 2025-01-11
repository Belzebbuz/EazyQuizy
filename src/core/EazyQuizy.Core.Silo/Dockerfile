FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8025
EXPOSE 8026

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/core/EazyQuizy.Core.Silo/EazyQuizy.Core.Silo.csproj", "src/core/EazyQuizy.Core.Silo/"]
COPY ["src/core/EazyQuizy.Core.Grains/EazyQuizy.Core.Grains.csproj", "src/core/EazyQuizy.Core.Grains/"]
COPY ["src/core/EazyQuizy.Core.Abstractions/EazyQuizy.Core.Abstractions.csproj", "src/core/EazyQuizy.Core.Abstractions/"]
COPY ["src/common/EazyQuizy.Common.Protos/EazyQuizy.Common.Protos.csproj", "src/common/EazyQuizy.Common.Protos/"]
COPY ["src/common/EazyQuizy.Common/EazyQuizy.Common.csproj", "src/common/EazyQuizy.Common/"]
COPY ["src/core/EazyQuizy.Core.Domain/EazyQuizy.Core.Domain.csproj", "src/core/EazyQuizy.Core.Domain/"]
RUN dotnet restore "src/core/EazyQuizy.Core.Silo/EazyQuizy.Core.Silo.csproj"
COPY . .
WORKDIR "/src/src/core/EazyQuizy.Core.Silo"
RUN dotnet build "EazyQuizy.Core.Silo.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EazyQuizy.Core.Silo.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EazyQuizy.Core.Silo.dll"]
