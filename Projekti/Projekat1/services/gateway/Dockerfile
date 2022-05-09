FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

ENV ASPNETCORE_ENVIRONMENT=Development

WORKDIR /src
COPY gateway.csproj .
COPY . .
RUN dotnet restore
RUN dotnet publish -c release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT [ "dotnet", "gateway.dll" ]