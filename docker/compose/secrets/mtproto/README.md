# RSA-ключи MTProto

Здесь должны лежать RSA-ключи сервера. Приватные ключи намеренно не хранятся в
репозитории — сгенерируйте свою пару перед запуском.

```bash
# Приватный ключ (PKCS#8)
openssl genrsa -out rsa_private.pem 2048

# Приватный ключ в формате PKCS#1 (используется сервером)
openssl rsa -in rsa_private.pem -traditional -out rsa_private_pkcs1.pem

# Публичный ключ
openssl rsa -in rsa_private.pem -pubout -out rsa_public.pem
```

После генерации в этом каталоге должны появиться файлы:

- `rsa_private.pem`
- `rsa_private_pkcs1.pem`
- `rsa_public.pem`

Эти файлы не коммитьте.
