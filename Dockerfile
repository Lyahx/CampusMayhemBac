FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY CampusMayhemApi/CampusMayhemApi.csproj CampusMayhemApi/
RUN dotnet restore CampusMayhemApi/CampusMayhemApi.csproj
COPY . .
RUN dotnet publish CampusMayhemApi/CampusMayhemApi.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "CampusMayhemApi.dll"]
