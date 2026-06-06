# Opengram

*Read this in other languages: [Русский](README.md)*

Opengram is a standalone Telegram server written in C# (.NET 9). The project is a fork of [mytelegram](https://github.com/loyldg/mytelegram) and implements the server side of the Telegram API (MTProto), which you can deploy on your own infrastructure.
Our Telegram channel: https://t.me/opengrame

## Features

- MTProto transports (Abridged, Intermediate), supports up to layer 216;
- private chats, groups, supergroups and channels;
- secret (end-to-end) chats;
- voice and video calls (via TURN/STUN and the mediasoup SFU);
- bots and the Bot API;
- privacy settings and two-factor authentication;
- stickers, reactions, custom emoji;
- Stars and Star Gifts, including resale and upgrade;
- Stories, themes and wallpapers;
- scheduled and self-destructing messages.

## Architecture

The server consists of a set of microservices that run via Docker Compose:

| Service | Purpose |
| --- | --- |
| `gateway-server` | Entry point for client MTProto connections |
| `auth-server` | Authorization and key exchange |
| `session-server` | Session storage and update routing |
| `messenger-command-server` | Command handling (write side, CQRS) |
| `messenger-query-server` | Query handling (read side, CQRS) |
| `bot-api-server` | HTTP Bot API |
| `admin-api` | Administration service API |
| `file-server` / `file-merge-proxy` | File storage and delivery |
| `turn-server` | TURN/STUN for calls |
| `sms-sender` | Sending verification codes |
| `data-seeder` | Initial database seeding |

Infrastructure: MongoDB (storage and event store), Redis (cache), RabbitMQ (event bus), MinIO (object storage for files).

Additional repository components:

- `mediasoup-server/` — SFU for group video calls (Node.js);
- `stargift-admin/` — web panel for managing gifts (Node.js backend, React frontend);
- `scripts/` — helper startup scripts and test bots.

## Quick start (Docker)

You need Docker and Docker Compose.

1. Go to the directory with the compose file:

   ```bash
   cd docker/compose
   ```

2. Open the `.env` file and set your own values in place of the `CHANGE_ME` placeholders
   (MongoDB, Redis, RabbitMQ, MinIO passwords, Admin API key), and specify the server's
   external IP address in the `App__WebRtcConnections` and `App__DcOptions` parameters.

3. Generate the MTProto RSA keys and place them in `docker/compose/secrets/mtproto/`
   (see `secrets/mtproto/README.md`).

4. Start the services:

   ```bash
   docker compose up -d
   ```

   Some services (including `messenger-query-server`) are built locally from source rather
   than pulled from a registry, so no `docker login` is required. The first run builds the
   images automatically. To rebuild them explicitly, run:

   ```bash
   docker compose build
   docker compose up -d
   ```

After startup, connect a Telegram client by pointing it to your data center address.

## Building from source

You need the .NET 9 SDK to build.

```bash
cd source
dotnet build MyTelegram.sln -c Release
```

The Docker image build scripts are located in the `build/` directory.

## Configuration

All settings are configured via environment variables (the `.env` file) or via each
service's `appsettings.json`. In the repository all password and key values are replaced
with `CHANGE_ME` placeholders — replace them with your own before starting.

Do not store real passwords or private keys in the repository.

## License and origin

The project is based on [mytelegram](https://github.com/loyldg/mytelegram). All rights to
the original code belong to its authors; respect the terms of the original project's license
and Telegram's trademarks.
