# Этап 1 — сборка приложения
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем только файл проекта и восстанавливаем зависимости отдельно
COPY ["practos3.csproj", "./"]
RUN dotnet restore "./practos3.csproj"

COPY . .
RUN dotnet publish "./practos3.csproj" -c Release -o /app/publish

# Этап 2 — минимальный runtime-образ без SDK
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Устанавливаем curl — используется для healthcheck в docker-compose.yml
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

# Приложение слушает на всех интерфейсах (0.0.0.0) на порту 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["dotnet", "practos3.dll"]
