# Магазин сувенирных чёток — REST API

ASP.NET Core 9.0 Minimal API для управления каталогом чёток. База данных — SQLite, документация — Swagger UI.

## Технологии

- .NET 9.0 / ASP.NET Core Minimal API
- Entity Framework Core 9 + SQLite
- Swagger (Swashbuckle)
- Docker / Docker Compose

## Запуск локально

```bash
dotnet run
```

Приложение будет доступно на `http://localhost:8080`.

## Запуск через Docker

```bash
docker build -t practos3 .
docker run -d -p 8080:8080 practos3
```

## Запуск через Docker Compose

```bash
docker-compose up -d
```

Приложение будет доступно на `http://localhost:8089`.  
Настройки порта и окружения задаются в файле `.env`.

## API

| Метод | Эндпоинт | Описание |
|---|---|---|
| GET | `/` | Информация о приложении |
| GET | `/api/products` | Список всех чёток |
| GET | `/api/products/{id}` | Чётки по ID |
| GET | `/api/categories` | Список категорий |
| GET | `/api/products/by-category/{id}` | Чётки по категории |
| POST | `/api/products` | Добавить товар |
| POST | `/api/categories` | Добавить категорию |
| GET | `/api/config` | Конфигурация приложения |
| GET | `/health` | Healthcheck |
| GET | `/swagger` | Swagger UI |

## Docker Hub

```bash
docker pull khachaturov/practos3:latest
```
