// Скрипт MongoDB: добавляет упрощённые темы оформления чата (только эмодзи-маркер)
// Сделано по образцу официального конструктора chatTheme из Telegram
// Запуск: docker exec -i compose-mongodb-1 mongosh tg

db = db.getSiblingDB('tg');

// Проверяем, не созданы ли темы оформления ранее
const existingCount = db.chat_themes.countDocuments();
if (existingCount > 0) {
    print(`Темы оформления уже есть (${existingCount}), удаляем...`);
    db.chat_themes.deleteMany({});
}

print('Добавляем упрощённые темы оформления чата (только эмодзи-маркер)...');

// Официальные темы оформления Telegram - только эмодзи-маркеры
const themes = [
    { _id: "1", Emoticon: "🏠" },  // Home
    { _id: "2", Emoticon: "❤️" },  // Love
    { _id: "3", Emoticon: "🎉" },  // Party
    { _id: "4", Emoticon: "🌊" },  // Ocean
    { _id: "5", Emoticon: "🌸" },  // Flower
    { _id: "6", Emoticon: "🌙" },  // Night
    { _id: "7", Emoticon: "🔥" },  // Fire
    { _id: "8", Emoticon: "💚" }   // Green
];

db.chat_themes.insertMany(themes);

print(`Добавлено упрощённых тем оформления чата: ${themes.length}`);
print(`Total chat themes in database: ${db.chat_themes.countDocuments()}`);
