# hallManagement

API для управління конференц-залами, бронюваннями та розрахунком вартості оренди.
Реалізовано як тестове завдання (Backend ТЗ).

## Бізнес-завдання

Компанія здає в оренду конференц-зали. Клієнтам потрібно:

1. Шукати вільні зали за датою, часом і місткістю.
2. Бронювати зал з обраними додатковими послугами (проєктор, Wi-Fi, звук).
3. Отримувати коректний розрахунок вартості, який враховує тарифні пояси доби
   (ранкова знижка, вечірня знижка, пікова націнка).

Адміністраторам потрібно:

1. Керувати каталогом залів (додавання/редагування/видалення).
2. Бачити аналітику — завантаженість залів, дохід, популярність послуг —
   щоб приймати рішення про ціноутворення та розширення парку залів.

## Технічний стек

- **.NET 9**, ASP.NET Core Web API
- **EF Core** + **PostgreSQL** (`Npgsql.EntityFrameworkCore.PostgreSQL`)
- **Swagger / Swashbuckle** — документація API
- **xUnit** — юніт-тести
- **Docker / Docker Compose** — локальний запуск без ручного встановлення .NET SDK чи Postgres

## Швидкий запуск (Docker) — рекомендований спосіб

Потрібен лише встановлений Docker.

```bash
git clone <URL-репозиторію>
cd <назва-папки-репозиторію>
docker compose up --build
```

Дочекайся рядка `Application started. Press Ctrl+C to shut down.` у логах `api`. Після цього:

- **Swagger UI:** http://localhost:8080/swagger

При першому старті контейнер `api` автоматично:

1. застосовує всі EF Core Migrations до бази (`db.Database.Migrate()`);
2. заповнює її початковими даними з ТЗ (Зал А/B/C, послуги Проєктор/Wi-Fi/Звук) через `DbSeeder`.

**Зупинити:**

```bash
docker compose down
```

**Зупинити й видалити дані БД** (для повністю чистого рестарту):

```bash
docker compose down -v
```

## Запуск без Docker (локально)

Потрібен **.NET 9 SDK** і піднятий Postgres.

```bash
# Postgres через Docker (лише база, без API)
docker run --name conference-booking-db \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=conference_booking \
  -p 5432:5432 -d postgres:16

cd src
dotnet restore
dotnet run
```

Рядок підключення береться з `appsettings.json` (`ConnectionStrings:Default`), за замовчуванням очікує Postgres на `localhost:5432`.

## Архітектура

Проєкт побудований за принципами Clean Code — з розділенням на шари за відповідальністю в межах одного проєкту:
