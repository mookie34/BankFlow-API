# ===== Etapa 1: Build =====
# Usa el SDK completo de .NET (pesado, tiene compilador)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar solo los .csproj primero (mejor cache de layers)
COPY BankFlow.sln .
COPY src/BankFlow.Domain/BankFlow.Domain.csproj src/BankFlow.Domain/
COPY src/BankFlow.Application/BankFlow.Application.csproj src/BankFlow.Application/
COPY src/BankFlow.Infrastructure/BankFlow.Infrastructure.csproj src/BankFlow.Infrastructure/
COPY src/BankFlow.API/BankFlow.API.csproj src/BankFlow.API/
COPY tests/BankFlow.UnitTests/BankFlow.UnitTests.csproj tests/BankFlow.UnitTests/
COPY nuget.config .

ENV DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER=0
ENV NUGET_CERT_REVOCATION_CHECK=false

RUN dotnet restore --disable-parallel

RUN dotnet restore

# Copiar todo el código fuente
COPY . .

# Correr los tests (si fallan, la imagen no se construye)
RUN dotnet test tests/BankFlow.UnitTests --no-restore --verbosity normal

# Publicar la API
RUN dotnet publish src/BankFlow.API -c Release -o /app --no-restore

# ===== Etapa 2: Runtime =====
# Usa solo el runtime de .NET (ligero, sin compilador)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Crear usuario sin privilegios (seguridad)
RUN adduser --disabled-password --gecos '' appuser

COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 5000

USER appuser
ENTRYPOINT ["dotnet", "BankFlow.API.dll"]