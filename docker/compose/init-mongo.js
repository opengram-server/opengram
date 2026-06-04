// Скрипт инициализации MongoDB
// Запускается при старте MongoDB с включённой аутентификацией
// Гарантирует, что пользователь admin существует с нужными правами

// Учтите: скрипт выполняется в контексте базы admin

// Создаём пользователя admin, если его ещё нет
try {
  db.createUser({
    user: "admin",
    pwd: "CHANGE_ME",
    roles: [
      { role: "root", db: "admin" },
      { role: "dbOwner", db: "tg" },
      { role: "readWrite", db: "tg" }
    ]
  });
  print("Пользователь admin создан");
} catch (e) {
  if (e.code === 48) {
    // Пользователь уже существует - это нормально
    print("Пользователь admin уже существует");
  } else {
    print("Ошибка при создании пользователя admin: " + e.message);
    throw e;
  }
}

// Создаём базу tg и обеспечиваем доступ к ней
db.getSiblingDB("tg").createCollection("dummy", { capped: false });
print("База 'tg' готова");
