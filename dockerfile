FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY BeerListApp.Api/BeerListApp.Api.csproj BeerListApp.Api/
RUN dotnet restore BeerListApp.Api/BeerListApp.Api.csproj

COPY BeerListApp.Api/ BeerListApp.Api/
RUN dotnet publish BeerListApp.Api/BeerListApp.Api.csproj \
    -c Release -o /out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /out .
RUN mkdir -p /data

ENTRYPOINT ["dotnet", "BeerListApp.Api.dll"]