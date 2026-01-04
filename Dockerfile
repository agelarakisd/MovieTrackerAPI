FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/MovieTracker.API/MovieTracker.API.csproj", "src/MovieTracker.API/"]
COPY ["src/MovieTracker.Application/MovieTracker.Application.csproj", "src/MovieTracker.Application/"]
COPY ["src/MovieTracker.Domain/MovieTracker.Domain.csproj", "src/MovieTracker.Domain/"]
COPY ["src/MovieTracker.Infrastructure/MovieTracker.Infrastructure.csproj", "src/MovieTracker.Infrastructure/"]

RUN dotnet restore "src/MovieTracker.API/MovieTracker.API.csproj"

COPY . .
WORKDIR "/src/src/MovieTracker.API"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MovieTracker.API.dll"]