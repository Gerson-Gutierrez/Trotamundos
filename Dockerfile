FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restaurar primero (aprovecha la cache de capas)
COPY Trotamundos.sln ./
COPY Trotamundos/Trotamundos.csproj Trotamundos/
COPY Datos/Datos.csproj Datos/
COPY Negocio/Negocio.csproj Negocio/
RUN dotnet restore Trotamundos/Trotamundos.csproj

COPY . .
RUN dotnet publish Trotamundos/Trotamundos.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
# Render inyecta PORT (10000 por defecto)
CMD ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-10000} exec dotnet Trotamundos.dll"]
