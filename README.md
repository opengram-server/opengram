# Opengram

*Читать на других языках: [English](README.en.md)*

Opengram — это самостоятельный сервер Telegram, написанный на C# (.NET 9). Проект является форком [mytelegram](https://github.com/loyldg/mytelegram) и реализует серверную часть API Telegram (MTProto), которую можно развернуть на собственной инфраструктуре.

[Telegram-канал](https://t.me/opengrame)

## Возможности

- MTProto-транспорты (Abridged, Intermediate), layer 216;
- личные чаты, группы, супергруппы и каналы;
- секретные (end-to-end) чаты;
- голосовые и видеозвонки (через TURN/STUN и SFU mediasoup);
- боты и Bot API;
- настройки приватности и двухфакторная аутентификация;
- стикеры, реакции, кастомные эмодзи;
- звёзды (Stars) и подарки (Star Gifts), включая перепродажу и апгрейд;
- истории (Stories), темы оформления и обои;
- запланированные и самоудаляющиеся сообщения.

## Архитектура

Сервер состоит из набора микросервисов, которые запускаются через Docker Compose:

| Сервис                             | Назначение                                                          |
|------------------------------------|---------------------------------------------------------------------|
| `gateway-server`                   | Точка входа для MTProto-подключений клиентов                        |
| `auth-server`                      | Авторизация и обмен ключами                                         |
| `session-server`                   | Хранение сессий и маршрутизация обновлений                          |
| `messenger-command-server`         | Обработка команд (запись, CQRS)                                     |
| `messenger-query-server`           | Обработка запросов (чтение, CQRS)                                   |
| `bot-api-server`                   | HTTP Bot API                                                        |
| `admin-api`                        | Служебный API администрирования                                     |
| `file-server`, `file-merge-proxy`  | Хранение и раздача файлов. К сожалению, код `file-server` закрытый. |
| `turn-server`                      | TURN/STUN для звонков                                               |
| `sms-sender`                       | Отправка кодов подтверждения                                        |
| `data-seeder`                      | Первичное наполнение базы данных                                    |

Инфраструктура: MongoDB (хранилище и event store), Redis (кеш), RabbitMQ (шина событий), MinIO (объектное хранилище файлов).

Дополнительные компоненты репозитория:

- `mediasoup-server/` — SFU для групповых видеозвонков (Node.js);
- `stargift-admin/` — веб-панель управления подарками (backend на Node.js, frontend на React);
- `scripts/` — вспомогательные скрипты запуска и тестовые боты.

## Быстрый старт (Docker)

Потребуются Docker и Docker Compose.

1. Перейдите в каталог с compose-файлом:

   ```bash
   cd docker/compose
   ```

2. Откройте файл `.env` и задайте свои значения вместо плейсхолдеров `CHANGE_ME`
   (пароли MongoDB, Redis, RabbitMQ, MinIO, ключ Admin API), а также укажите внешний
   IP-адрес сервера в параметрах `App__WebRtcConnections` и `App__DcOptions`.

3. Сгенерируйте RSA-ключи MTProto командами:

   ```bash
   cd docker/compose/secrets/mtproto

   # Приватный ключ (PKCS#8)
   openssl genrsa -out rsa_private.pem 2048

   # Приватный ключ в формате PKCS#1 (используется сервером)
   openssl rsa -in rsa_private.pem -traditional -out rsa_private_pkcs1.pem

   # Публичный ключ
   openssl rsa -in rsa_private.pem -pubout -out rsa_public.pem
   ```

   Приватные ключи намеренно не хранятся в
   репозитории — сгенерируйте свою пару перед запуском.

4. Сгенерируйте самоподписанный SSL-сертификат:

   ```bash
   openssl req -new -x509 -key rsa_private.pem -out rsa_certificate.pem -days 3650 -subj '/CN=telegram.server'
   ```

5. Запустите сервисы:

   ```bash
   docker compose up --build
   ```

После старта подключите клиент Telegram, прописав адрес вашего дата-центра.

## Сборка из исходников

Для сборки нужен .NET 9 SDK.

```bash
cd source
dotnet build MyTelegram.sln -c Release
```

Скрипты сборки Docker-образов лежат в каталоге `build/`.

## Конфигурация

Все настройки задаются через переменные окружения (файл `.env`) либо через
`appsettings.json` отдельных сервисов. В репозитории все значения паролей и ключей
заменены на плейсхолдеры `CHANGE_ME` — перед запуском замените их на свои.

Не храните реальные пароли и приватные ключи в репозитории.

## Лицензия и происхождение

Проект основан на [mytelegram](https://github.com/loyldg/mytelegram). Все права на
оригинальный код принадлежат его авторам; уважайте условия лицензии исходного проекта
и товарные знаки Telegram.

## Нюансы

К сожалению, код микросервиса `file-server` закрытый. Я не знаю что внутри, там может быть бэкдор или все что угодно. В целях безопасности я отключил этому микросервису доступ к интернету. Это решение автора MyTelegram, мы не виноваты.