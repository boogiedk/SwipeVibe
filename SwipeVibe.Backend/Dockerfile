FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SwipeVibe.Backend/SwipeVibe.Backend.csproj", "SwipeVibe.Backend/"]
RUN dotnet restore "SwipeVibe.Backend/SwipeVibe.Backend.csproj"
COPY . .
WORKDIR "/src/SwipeVibe.Backend"
RUN dotnet build "SwipeVibe.Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SwipeVibe.Backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SwipeVibe.Backend.dll"]
