FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DistributedCache/DistributedCache.csproj", "DistributedCache/"]
RUN dotnet restore "DistributedCache/DistributedCache.csproj"
COPY . .
WORKDIR "/src/DistributedCache"
RUN dotnet build "DistributedCache.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DistributedCache.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DistributedCache.dll"]
