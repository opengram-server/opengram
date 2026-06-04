// Скрипт MongoDB: добавляет темы оформления чата, используя Unicode-escape-последовательности
// Так удаётся избежать проблем с кодировкой при передаче через docker exec
// Запуск: docker exec -i compose-mongodb-1 mongosh tg < seed-chat-themes-unicode.js

db = db.getSiblingDB('tg');

// Удаляем существующие темы
print('Удаляем существующие темы оформления чата...');
db.chat_themes.deleteMany({});

print('Добавляем темы оформления через Unicode-escape...');

// Используем Unicode-escape-последовательности, чтобы избежать проблем с кодировкой
const themes = [
    { _id: "1", Emoticon: "\uD83C\uDFE0" },  // Home
    { _id: "2", Emoticon: "\u2764\uFE0F" },  // Love
    { _id: "3", Emoticon: "\uD83C\uDF89" },  // Party
    { _id: "4", Emoticon: "\uD83C\uDF0A" },  // Ocean
    { _id: "5", Emoticon: "\uD83C\uDF38" },  // Flower
    { _id: "6", Emoticon: "\uD83C\uDF19" },  // Night
    { _id: "7", Emoticon: "\uD83D\uDD25" },  // Fire
    { _id: "8", Emoticon: "\uD83D\uDC9A" }   // Green
];

db.chat_themes.insertMany(themes);

print(`Добавлено тем оформления чата: ${themes.length}`);
print('Проверяем темы:');
db.chat_themes.find().forEach(function(theme) {
    print(`  ID: ${theme._id}, Emoticon: ${theme.Emoticon}`);
});
