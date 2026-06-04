# Star Gifts Admin Panel

Веб-панель для управления подарками (Star Gifts) в Opengram. Состоит из backend на
Node.js и frontend на React. Проект организован как npm-workspace.

## Возможности

- панель с аналитикой и статистикой;
- создание, редактирование и удаление подарков;
- загрузка анимаций (JSON/TGS);
- фильтрация и поиск;
- обновление данных в реальном времени.

## Быстрый старт

### 1. Установка зависимостей

```bash
cd stargift-admin
npm run install:all
```

### 2. База данных

Убедитесь, что MongoDB запущена. Её можно поднять из основного compose-файла:

```bash
docker-compose -f ../docker/compose/docker-compose.yml up mongodb -d
```

### 3. Запуск в режиме разработки

```bash
npm run dev
```

Панель будет доступна по адресу http://localhost:5173, backend — на порту 3001.

## Структура

```
stargift-admin/
├── backend/          # Node.js API
│   └── src/
│       ├── routes/
│       └── server.js
├── frontend/         # React UI
│   └── src/
└── package.json      # npm-workspace
```

## Основные эндпоинты API

- `GET /api/gifts` — список подарков;
- `POST /api/gifts` — создать подарок;
- `PUT /api/gifts/:id` — обновить подарок;
- `DELETE /api/gifts/:id` — удалить подарок;
- `GET /api/stats` — статистика.

## Конфигурация

Настройки backend задаются в `backend/.env`. Перед запуском замените значения
паролей и ключей на свои (в репозитории они заменены на плейсхолдеры).

```
MONGODB_URI=mongodb://localhost:27017
DB_NAME=tg
PORT=3001
```

## Лицензия

MIT
