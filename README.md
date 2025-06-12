# 🛒 Shop Microservices

Микросервисное приложение для онлайн-магазина с автосписанием средств при оформлении заказа. Проект реализован на .NET 8 с использованием Clean Architecture, MassTransit, RabbitMQ, PostgreSQL, Docker и YARP API Gateway.

## 📦 Сервисы

- **OrderService**
  - Создание заказа
  - Получение заказов пользователя
  - Получение статуса заказа
  - Публикация задач оплаты в Outbox

- **PaymentsService**
  - Создание счёта
  - Пополнение счёта
  - Получение баланса
  - Inbox-consumer для задач оплаты
  - Outbox-публикация результата оплаты

- **APIGateway**
  - YARP reverse proxy
  - Проксирует все запросы `/api/orders/**` и `/api/accounts/**`

## 🧱 Архитектура

- Взаимодействие между сервисами через RabbitMQ
- Используются паттерны Transactional Outbox / Inbox для гарантированной доставки
- Хранение данных: PostgreSQL
- Документация API: встроенный Swagger в каждом сервисе

## 🚀 Запуск через Docker

1. Построить и запустить:

   ```bash
   docker compose up --build
   ```

2. Доступные порты:

   - `localhost:8000` — API Gateway (точка входа)
   - `localhost:6001` — Swagger UI OrderService
   - `localhost:6002` — Swagger UI PaymentsService
   - `localhost:15672` — RabbitMQ Management UI (логин: guest / пароль: guest)
   - `localhost:5432` — PostgreSQL

## 🔀 API Endpoints (через API Gateway)

> Базовый адрес: `http://localhost:8000`

### 📦 Orders

- `POST /api/orders`  
  Создать заказ  
  **Body:**
  ```json
  {
    "userId": "uuid",
    "amount": 100,
    "description": "Описание заказа"
  }
  ```

- `GET /api/orders/user/{userId}`  
  Получить все заказы пользователя

- `GET /api/orders/{orderId}/status`  
  Получить статус заказа: `"New"`, `"Finished"`, `"Cancelled"`

### 💳 Accounts

- `POST /api/accounts/{userId}`  
  Создать счёт

- `POST /api/accounts/{userId}/deposit`  
  Пополнить счёт  
  **Body:**
  ```json
  {
    "amount": 1000
  }
  ```

- `GET /api/accounts/{userId}/balance`  
  Получить баланс пользователя

## 🧪 Тестирование

1. Swagger UI каждого сервиса доступен по портам `6001` и `6002`
2. Через API Gateway можно тестировать Postman-коллекцией или curl-запросами
3. Примеры доступны в разделе “API Endpoints”

## 🛠 Технологии

- ASP.NET Core 8
- MassTransit + RabbitMQ
- PostgreSQL
- Entity Framework Core
- Docker + Docker Compose
- YARP (Reverse Proxy)
- Swagger / OpenAPI
