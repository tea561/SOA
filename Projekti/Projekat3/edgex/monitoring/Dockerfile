FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

ENV ASPNETCORE_ENVIRONMENT=Development

WORKDIR /src
EXPOSE 80
COPY monitoring.csproj .
COPY . .
RUN dotnet restore
RUN dotnet publish -c release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT [ "dotnet", "monitoring.dll" ]