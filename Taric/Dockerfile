FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Taric/Taric.csproj", "Taric/"]
RUN dotnet restore "Taric/Taric.csproj"
COPY . .
WORKDIR "/src/Taric"
RUN dotnet build "Taric.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Taric.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Taric.dll"]
