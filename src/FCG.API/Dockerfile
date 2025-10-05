# Fase de build da aplicação, onde usa boas práticas de restore para criar mais rápido o artefato. Executa como root para instalar dependências.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
USER root
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FCG.API/FCG.API.csproj", "FCG.API/"]
RUN dotnet restore "./FCG.API/FCG.API.csproj"
COPY . .
WORKDIR "/src/FCG.API"
RUN dotnet build "./FCG.API.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet publish "./FCG.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Fase de deploy da aplicação, onde apenas copia o artefato do publish, altera usuário para non-root e executa o entrypoint
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS deploy
WORKDIR /app
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "FCG.API.dll"]
