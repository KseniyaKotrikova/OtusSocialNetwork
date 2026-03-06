# OTUS Highload Architect: Социальная сеть (Скелет)

Базовая реализация бэкенда социальной сети на **.NET 8** с использованием **Postgres** (без ORM). Проект разработан как фундамент для дальнейшего внедрения репликации, шардирования и кэширования.

## 🚀 Особенности реализации
- **Архитектура:** Монолит с разделением на слои (`Controllers`, `Repositories`, `Models`, `Filters`).
- **Data Access:** Использование сырого SQL через **Npgsql** (ADO.NET). Полный отказ от ORM для максимального контроля производительности.
- **Безопасность:**
    - Хеширование паролей с помощью **BCrypt**.
    - Token-based авторизация через кастомный `ActionFilter`.
    - Хранение сессий в памяти (`ConcurrentDictionary`).
- **Docker:** Оптимизированный образ на базе **Alpine + Node.js** для обхода ограничений стандартных реестров Microsoft.

## 🛠 Технологический стек
- **Runtime:** .NET 8.0 (C#)
- **Database:** PostgreSQL 15
- **Docs:** Swagger (OpenAPI 3.0)
- **Containerization:** Docker Compose

## 📦 Инструкция по запуску

### ⚙️ 1. Сборка приложения (Local Publish)
Из-за использования легковесного рантайма в Docker, необходимо предварительно собрать бинарные файлы под целевую платформу:
```bash
dotnet clean
dotnet publish -c Release -f net8.0 -o ./publish
```
### 🚀2. Запуск приложения
```bash
docker-compose up --build
```
### 🎯3. ТЕСТИРОВАНИЕ
## Регистрация:

```bash
curl -X POST http://localhost:5000/user/register \
-H "Content-Type: application/json" \
-d '{"first_name":"Иван","second_name":"Иванов","birthdate":"1990-01-01","biography":"Highload","city":"MSK","password":"123","gender":"male"}'
```

## Логин (получение токена):
```bash
curl -X POST http://localhost:5000/login \
-H "Content-Type: application/json" \
-d '{"id":"ВАШ_GUID","password":"123"}'
```
## Получение данных:
```bash
curl -H "Authorization: Bearer ВАШ_ТОКЕН" http://localhost:5000/user/get/ВАШ_GUID
```
