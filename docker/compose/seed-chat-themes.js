// Скрипт MongoDB: добавляет стандартные темы оформления чата вместе с обоями
// Запуск: docker exec -i compose-mongodb-1 mongosh tg

db = db.getSiblingDB('tg');

// Проверяем, не созданы ли темы оформления ранее
const existingCount = db.chat_themes.countDocuments();
if (existingCount > 0) {
    print(`Chat themes already exist (${existingCount}), skipping...`);
} else {
    print('Добавляем стандартные темы оформления чата...');
    
    const themes = [
        {
            _id: "1",
            Id: NumberLong("1"),
            AccessHash: NumberLong("1234567890123456"),
            Slug: "blue",
            Title: "Blue",
            Default: true,
            ForChat: true,
            Emoticon: "💙",
            DocumentId: null,
            Settings: [
                {
                    MessageColorsAnimated: false,
                    BaseTheme: NumberLong("2"), // Дневная
                    AccentColor: 0x0f4c81,
                    OutboxAccentColor: null,
                    MessageColors: [0x0f4c81, 0x1a7fc1],
                    WallPaper: {
                        Id: NumberLong("1001"),
                        AccessHash: NumberLong("1001"),
                        Default: true,
                        Pattern: false,
                        Dark: false,
                        Slug: "blue-gradient",
                        DocumentId: null,
                        Settings: {
                            Blur: false,
                            Motion: true,
                            BackgroundColor: 0x0f4c81,
                            SecondBackgroundColor: 0x1a7fc1,
                            ThirdBackgroundColor: null,
                            FourthBackgroundColor: null,
                            Intensity: 50,
                            Rotation: 45,
                            Emoticon: "💙"
                        }
                    }
                }
            ]
        },
        {
            _id: "2",
            Id: NumberLong("2"),
            AccessHash: NumberLong("1234567890123457"),
            Slug: "pink",
            Title: "Pink",
            Default: true,
            ForChat: true,
            Emoticon: "💗",
            DocumentId: null,
            Settings: [
                {
                    MessageColorsAnimated: false,
                    BaseTheme: NumberLong("2"),
                    AccentColor: 0xff6b9d,
                    OutboxAccentColor: null,
                    MessageColors: [0xff6b9d, 0xffa8c5],
                    WallPaper: {
                        Id: NumberLong("1002"),
                        AccessHash: NumberLong("1002"),
                        Default: true,
                        Pattern: false,
                        Dark: false,
                        Slug: "pink-gradient",
                        DocumentId: null,
                        Settings: {
                            Blur: false,
                            Motion: true,
                            BackgroundColor: 0xff6b9d,
                            SecondBackgroundColor: 0xffa8c5,
                            ThirdBackgroundColor: null,
                            FourthBackgroundColor: null,
                            Intensity: 50,
                            Rotation: 135,
                            Emoticon: "💗"
                        }
                    }
                }
            ]
        },
        {
            _id: "3",
            Id: NumberLong("3"),
            AccessHash: NumberLong("1234567890123458"),
            Slug: "green",
            Title: "Green",
            Default: true,
            ForChat: true,
            Emoticon: "💚",
            DocumentId: null,
            Settings: [
                {
                    MessageColorsAnimated: false,
                    BaseTheme: NumberLong("2"),
                    AccentColor: 0x0a8754,
                    OutboxAccentColor: null,
                    MessageColors: [0x0a8754, 0x2ecc71],
                    WallPaper: {
                        Id: NumberLong("1003"),
                        AccessHash: NumberLong("1003"),
                        Default: true,
                        Pattern: false,
                        Dark: false,
                        Slug: "green-gradient",
                        DocumentId: null,
                        Settings: {
                            Blur: false,
                            Motion: true,
                            BackgroundColor: 0x0a8754,
                            SecondBackgroundColor: 0x2ecc71,
                            ThirdBackgroundColor: null,
                            FourthBackgroundColor: null,
                            Intensity: 50,
                            Rotation: 90,
                            Emoticon: "💚"
                        }
                    }
                }
            ]
        },
        {
            _id: "4",
            Id: NumberLong("4"),
            AccessHash: NumberLong("1234567890123459"),
            Slug: "orange",
            Title: "Orange",
            Default: true,
            ForChat: true,
            Emoticon: "🧡",
            DocumentId: null,
            Settings: [
                {
                    MessageColorsAnimated: false,
                    BaseTheme: NumberLong("2"),
                    AccentColor: 0xff6b35,
                    OutboxAccentColor: null,
                    MessageColors: [0xff6b35, 0xffa500],
                    WallPaper: {
                        Id: NumberLong("1004"),
                        AccessHash: NumberLong("1004"),
                        Default: true,
                        Pattern: false,
                        Dark: false,
                        Slug: "orange-gradient",
                        DocumentId: null,
                        Settings: {
                            Blur: false,
                            Motion: true,
                            BackgroundColor: 0xff6b35,
                            SecondBackgroundColor: 0xffa500,
                            ThirdBackgroundColor: null,
                            FourthBackgroundColor: null,
                            Intensity: 50,
                            Rotation: 180,
                            Emoticon: "🧡"
                        }
                    }
                }
            ]
        },
        {
            _id: "5",
            Id: NumberLong("5"),
            AccessHash: NumberLong("1234567890123460"),
            Slug: "purple",
            Title: "Purple",
            Default: true,
            ForChat: true,
            Emoticon: "💜",
            DocumentId: null,
            Settings: [
                {
                    MessageColorsAnimated: false,
                    BaseTheme: NumberLong("2"),
                    AccentColor: 0x8e44ad,
                    OutboxAccentColor: null,
                    MessageColors: [0x8e44ad, 0xc39bd3],
                    WallPaper: {
                        Id: NumberLong("1005"),
                        AccessHash: NumberLong("1005"),
                        Default: true,
                        Pattern: false,
                        Dark: false,
                        Slug: "purple-gradient",
                        DocumentId: null,
                        Settings: {
                            Blur: false,
                            Motion: true,
                            BackgroundColor: 0x8e44ad,
                            SecondBackgroundColor: 0xc39bd3,
                            ThirdBackgroundColor: null,
                            FourthBackgroundColor: null,
                            Intensity: 50,
                            Rotation: 225,
                            Emoticon: "💜"
                        }
                    }
                }
            ]
        },
        {
            _id: "6",
            Id: NumberLong("6"),
            AccessHash: NumberLong("1234567890123461"),
            Slug: "sunset",
            Title: "Sunset",
            Default: true,
            ForChat: true,
            Emoticon: "🌅",
            DocumentId: null,
            Settings: [
                {
                    MessageColorsAnimated: true,
                    BaseTheme: NumberLong("2"),
                    AccentColor: 0xff6b35,
                    OutboxAccentColor: null,
                    MessageColors: [0xff6b35, 0xff8c42, 0xffa94d, 0xffc65c],
                    WallPaper: {
                        Id: NumberLong("1006"),
                        AccessHash: NumberLong("1006"),
                        Default: true,
                        Pattern: false,
                        Dark: false,
                        Slug: "sunset-gradient",
                        DocumentId: null,
                        Settings: {
                            Blur: false,
                            Motion: true,
                            BackgroundColor: 0xff6b35,
                            SecondBackgroundColor: 0xff8c42,
                            ThirdBackgroundColor: 0xffa94d,
                            FourthBackgroundColor: 0xffc65c,
                            Intensity: 60,
                            Rotation: 315,
                            Emoticon: "🌅"
                        }
                    }
                }
            ]
        },
        {
            _id: "7",
            Id: NumberLong("7"),
            AccessHash: NumberLong("1234567890123462"),
            Slug: "ocean",
            Title: "Ocean",
            Default: true,
            ForChat: true,
            Emoticon: "🌊",
            DocumentId: null,
            Settings: [
                {
                    MessageColorsAnimated: true,
                    BaseTheme: NumberLong("2"),
                    AccentColor: 0x006994,
                    OutboxAccentColor: null,
                    MessageColors: [0x006994, 0x0099cc, 0x33b5e5, 0x66ccff],
                    WallPaper: {
                        Id: NumberLong("1007"),
                        AccessHash: NumberLong("1007"),
                        Default: true,
                        Pattern: false,
                        Dark: false,
                        Slug: "ocean-gradient",
                        DocumentId: null,
                        Settings: {
                            Blur: false,
                            Motion: true,
                            BackgroundColor: 0x006994,
                            SecondBackgroundColor: 0x0099cc,
                            ThirdBackgroundColor: 0x33b5e5,
                            FourthBackgroundColor: 0x66ccff,
                            Intensity: 60,
                            Rotation: 0,
                            Emoticon: "🌊"
                        }
                    }
                }
            ]
        }
    ];
    
    db.chat_themes.insertMany(themes);
    
    print(`Добавлено тем оформления чата: ${themes.length}`);
}

// Выводим итоговое количество
print(`Total chat themes in database: ${db.chat_themes.countDocuments()}`);
