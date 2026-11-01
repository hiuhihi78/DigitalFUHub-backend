# ---------- Build ----------
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Restore first (cached unless a .csproj changes)
COPY BusinessObject/BusinessObject.csproj BusinessObject/
COPY Comons/Comons.csproj Comons/
COPY DataAccess/DataAccess.csproj DataAccess/
COPY DTOs/DTOs.csproj DTOs/
COPY DigitalFUHubApi/DigitalFUHubApi.csproj DigitalFUHubApi/
RUN dotnet restore DigitalFUHubApi/DigitalFUHubApi.csproj

COPY . .
RUN dotnet publish DigitalFUHubApi/DigitalFUHubApi.csproj -c Release -o /app/publish --no-restore

# ---------- Runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "DigitalFUHubApi.dll"]
