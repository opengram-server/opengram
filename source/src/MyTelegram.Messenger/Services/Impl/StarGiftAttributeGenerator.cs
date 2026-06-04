using MyTelegram.Schema;
using MyTelegram.Messenger.Services.Interfaces;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Linq;

namespace MyTelegram.Messenger.Services.Impl;

public class StarGiftAttributeGenerator : IStarGiftAttributeGenerator
{
    private readonly IRandomHelper _randomHelper;
    private readonly IMongoDatabase _mongoDatabase;

    public StarGiftAttributeGenerator(IRandomHelper randomHelper, IMongoDatabase mongoDatabase)
    {
        _randomHelper = randomHelper;
        _mongoDatabase = mongoDatabase;
    }

    public TVector<IStarGiftAttribute> GenerateAttributes(long giftId)
    {
        var attributes = new TVector<IStarGiftAttribute>();
        
        try
        {
            // Пробуем загрузить набор атрибутов из базы данных
            var collection = _mongoDatabase.GetCollection<BsonDocument>("StarGiftAttributeSets");
            var filter = Builders<BsonDocument>.Filter.Eq("GiftId", giftId);
            var attributeSet = collection.Find(filter).FirstOrDefault();

            if (attributeSet != null)
            {
                Console.WriteLine($"[StarGiftAttributeGenerator] Found attribute set for gift {giftId}");

                // Загружаем Model
                if (attributeSet.Contains("Model") && !attributeSet["Model"].IsBsonNull)
                {
                    var model = attributeSet["Model"].AsBsonDocument;
                    var docId = model.GetValue("DocumentId", 0L).ToInt64();
                    var document = LoadDocumentFromDb(docId);
                    
                    if (document != null)
                    {
                        attributes.Add(new TStarGiftAttributeModel
                        {
                            Name = attributeSet.GetValue("ModelName", attributeSet.GetValue("Name", "Custom Model").AsString).AsString,
                            Document = document,
                            RarityPermille = model.GetValue("RarityPermille", 1000).ToInt32()
                        });
                    }
                }
                
                // Загружаем Pattern
                if (attributeSet.Contains("Pattern") && !attributeSet["Pattern"].IsBsonNull)
                {
                    var pattern = attributeSet["Pattern"].AsBsonDocument;
                    var docId = pattern.GetValue("DocumentId", 0L).ToInt64();
                    var document = LoadDocumentFromDb(docId);
                    
                    if (document != null)
                    {
                        attributes.Add(new TStarGiftAttributePattern
                        {
                            Name = attributeSet.GetValue("PatternName", attributeSet.GetValue("Name", "Custom Pattern").AsString).AsString,
                            Document = document,
                            RarityPermille = pattern.GetValue("RarityPermille", 1000).ToInt32()
                        });
                    }
                }
                
                // Загружаем Backdrop
                if (attributeSet.Contains("Backdrop") && !attributeSet["Backdrop"].IsBsonNull)
                {
                    var backdrop = attributeSet["Backdrop"].AsBsonDocument;

                    // Получаем BackdropId: он хранится как long, а нам нужен int
                    var backdropIdValue = backdrop.Contains("BackdropId")
                        ? backdrop.GetValue("BackdropId", (long)giftId).ToInt32() 
                        : (int)giftId;
                    
                    attributes.Add(new TStarGiftAttributeBackdrop
                    {
                        Name = attributeSet.GetValue("BackdropName", attributeSet.GetValue("Name", "Custom Backdrop").AsString).AsString,
                        BackdropId = backdropIdValue,
                        CenterColor = backdrop.GetValue("CenterColor", 0x007AFF).ToInt32(),
                        EdgeColor = backdrop.GetValue("EdgeColor", 0x0A84FF).ToInt32(),
                        PatternColor = backdrop.GetValue("PatternColor", 0x5AC8FA).ToInt32(),
                        TextColor = backdrop.GetValue("TextColor", 0xFFFFFF).ToInt32(),
                        RarityPermille = backdrop.GetValue("RarityPermille", 1000).ToInt32()
                    });
                }
                
                // Клиенту нужны все три типа атрибутов (Model, Pattern, Backdrop).
                // Если backdrop отсутствует, добавляем случайный из пула, чтобы в превью было разнообразие
                var hasBackdrop = attributes.Any(a => a is TStarGiftAttributeBackdrop);
                if (!hasBackdrop)
                {
                    Console.WriteLine($"[StarGiftAttributeGenerator] Adding random backdrop for gift {giftId}");
                    // Используем идентификатор подарка как seed, чтобы в превью каждый раз были разные фоны
                    var randomId = Guid.NewGuid().ToString();
                    attributes.Add(GenerateRandomBackdrop(randomId));
                }

                if (attributes.Count > 0)
                {
                    Console.WriteLine($"[StarGiftAttributeGenerator] Loaded {attributes.Count} attributes from database");
                    return attributes;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[StarGiftAttributeGenerator] Error loading attributes: {ex.Message}");
        }

        // Если атрибуты не найдены, возвращаем заглушку
        Console.WriteLine($"[StarGiftAttributeGenerator] Using placeholder attributes for gift {giftId}");
        return GeneratePlaceholderAttributes(giftId);
    }
    
    private TDocument? LoadDocumentFromDb(long documentId)
    {
        try
        {
            var collection = _mongoDatabase.GetCollection<BsonDocument>("ReadModel-DocumentReadModel");
            var filter = Builders<BsonDocument>.Filter.Eq("DocumentId", documentId);
            var doc = collection.Find(filter).FirstOrDefault();
            
            if (doc == null)
            {
                Console.WriteLine($"[StarGiftAttributeGenerator] Document {documentId} not found in ReadModel-DocumentReadModel");
                return null;
            }

            Console.WriteLine($"[StarGiftAttributeGenerator] Document {documentId} loaded successfully");
            
            var document = new TDocument
            {
                Id = doc.GetValue("DocumentId", documentId).ToInt64(),
                AccessHash = doc.GetValue("AccessHash", 0L).ToInt64(),
                FileReference = doc.GetValue("FileReference", BsonBinaryData.Create(Array.Empty<byte>())).AsByteArray,
                Date = doc.GetValue("Date", 0).ToInt32(),
                MimeType = doc.GetValue("MimeType", "application/x-tgsticker").AsString,
                Size = doc.GetValue("Size", 0L).ToInt64(),
                DcId = doc.GetValue("DcId", 2).ToInt32(),
                Attributes = new TVector<IDocumentAttribute>(
                    new TDocumentAttributeSticker
                    {
                        Alt = "🎁",
                        Stickerset = new TInputStickerSetEmpty()
                    },
                    new TDocumentAttributeAnimated()
                )
            };
            
            // ComputeFlag() обязательно вызвать перед сериализацией
            document.ComputeFlag();

            return document;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[StarGiftAttributeGenerator] Error loading document {documentId}: {ex.Message}");
            return null;
        }
    }
    
    private TVector<IStarGiftAttribute> GeneratePlaceholderAttributes(long giftId)
    {
        var attributes = new TVector<IStarGiftAttribute>();
        
        // Возвращаем ТОЛЬКО атрибут Backdrop без Model/Pattern, чтобы избежать ошибок разбора.
        // Model и Pattern требуют валидных документов с корректными file reference,
        // а Backdrop работает самостоятельно, ему достаточно только цветов

        attributes.Add(new TStarGiftAttributeBackdrop
        {
            Name = "Default Backdrop",
            BackdropId = (int)giftId,
            CenterColor = 0x007AFF,  // Синий
            EdgeColor = 0x0A84FF,    // Светло-синий
            PatternColor = 0x5AC8FA, // Голубой
            TextColor = 0xFFFFFF,    // Белый
            RarityPermille = 1000
        });

        Console.WriteLine($"[StarGiftAttributeGenerator] Using minimal placeholder (backdrop only) for gift {giftId}");
        
        return attributes;
    }

    /// <summary>
    /// Генерирует уникальные случайные атрибуты для улучшенного подарка.
    /// Случайный выбор учитывает RarityPermille (вероятность в промилле)
    /// </summary>
    public TVector<IStarGiftAttribute> GenerateUpgradeAttributes(long giftId, string uniqueId)
    {
        var attributes = new TVector<IStarGiftAttribute>();

        // uniqueId служит базовым seed, но для каждого типа атрибута добавляем свой сдвиг.
        // Так каждый атрибут получает свою последовательность случайных чисел
        var baseSeed = uniqueId.GetHashCode();

        // Сначала пробуем загрузить собственные атрибуты из базы
        try
        {
            var collection = _mongoDatabase.GetCollection<BsonDocument>("StarGiftAttributeSets");
            var filter = Builders<BsonDocument>.Filter.Eq("GiftId", giftId);

            // Загружаем ВСЕ наборы атрибутов для этого подарка (а не только первый)
            var allAttributeSets = collection.Find(filter).ToList();

            if (allAttributeSets.Count > 0)
            {
                Console.WriteLine($"[StarGiftAttributeGenerator] Found {allAttributeSets.Count} upgrade attribute sets for gift {giftId}");

                // Выбираем Model случайно с учётом RarityPermille
                var allModels = new List<(BsonDocument set, BsonDocument model, int rarity)>();
                foreach (var attrSet in allAttributeSets)
                {
                    if (attrSet.Contains("Model") && !attrSet["Model"].IsBsonNull)
                    {
                        var model = attrSet["Model"].AsBsonDocument;
                        var rarity = model.GetValue("RarityPermille", 1000).ToInt32();
                        allModels.Add((attrSet, model, rarity));
                    }
                }
                
                if (allModels.Count > 0)
                {
                    // Создаём отдельный Random для Model со своим сдвигом seed
                    var modelRandom = new Random(baseSeed + 1);
                    var selectedModel = SelectByRarity(allModels, modelRandom);
                    if (selectedModel.model != null)
                    {
                        var docId = selectedModel.model.GetValue("DocumentId", 0L).ToInt64();
                        var document = LoadDocumentFromDb(docId);

                        if (document != null)
                        {
                            var modelName = selectedModel.set.GetValue("ModelName", selectedModel.set.GetValue("Name", "Upgraded Model").AsString + " Model").AsString;
                            Console.WriteLine($"[StarGiftAttributeGenerator] Selected Model: {modelName} (DocumentId={docId}, Rarity={selectedModel.rarity}‰)");
                            
                            attributes.Add(new TStarGiftAttributeModel
                            {
                                Name = modelName,
                                Document = document,
                                RarityPermille = selectedModel.rarity
                            });
                        }
                    }
                }
                
                // Выбираем Pattern случайно с учётом RarityPermille
                var allPatterns = new List<(BsonDocument set, BsonDocument pattern, int rarity)>();
                foreach (var attrSet in allAttributeSets)
                {
                    if (attrSet.Contains("Pattern") && !attrSet["Pattern"].IsBsonNull)
                    {
                        var pattern = attrSet["Pattern"].AsBsonDocument;
                        var rarity = pattern.GetValue("RarityPermille", 1000).ToInt32();
                        allPatterns.Add((attrSet, pattern, rarity));
                    }
                }
                
                if (allPatterns.Count > 0)
                {
                    // Создаём отдельный Random для Pattern с другим сдвигом seed
                    var patternRandom = new Random(baseSeed + 2);
                    var selectedPattern = SelectByRarity(allPatterns, patternRandom);
                    if (selectedPattern.pattern != null)
                    {
                        var docId = selectedPattern.pattern.GetValue("DocumentId", 0L).ToInt64();
                        var document = LoadDocumentFromDb(docId);

                        if (document != null)
                        {
                            var patternName = selectedPattern.set.GetValue("PatternName", selectedPattern.set.GetValue("Name", "Unique Pattern").AsString + " Pattern").AsString;
                            Console.WriteLine($"[StarGiftAttributeGenerator] Selected Pattern: {patternName} (DocumentId={docId}, Rarity={selectedPattern.rarity}‰)");
                            
                            attributes.Add(new TStarGiftAttributePattern
                            {
                                Name = patternName,
                                Document = document,
                                RarityPermille = selectedPattern.rarity
                            });
                        }
                    }
                }
                
                // Выбираем Backdrop случайно с учётом RarityPermille
                var allBackdrops = new List<(BsonDocument set, BsonDocument backdrop, int rarity)>();
                foreach (var attrSet in allAttributeSets)
                {
                    if (attrSet.Contains("Backdrop") && !attrSet["Backdrop"].IsBsonNull)
                    {
                        var backdrop = attrSet["Backdrop"].AsBsonDocument;
                        var rarity = backdrop.GetValue("RarityPermille", 1000).ToInt32();
                        allBackdrops.Add((attrSet, backdrop, rarity));
                    }
                }
                
                if (allBackdrops.Count > 0)
                {
                    // Создаём отдельный Random для Backdrop с ещё одним сдвигом seed
                    var backdropRandom = new Random(baseSeed + 3);
                    var selectedBackdrop = SelectByRarity(allBackdrops, backdropRandom);
                    if (selectedBackdrop.backdrop != null)
                    {
                        var backdropIdValue = selectedBackdrop.backdrop.Contains("BackdropId")
                            ? selectedBackdrop.backdrop.GetValue("BackdropId", (long)giftId).ToInt32()
                            : (int)giftId;

                        var backdropName = selectedBackdrop.set.GetValue("BackdropName", selectedBackdrop.set.GetValue("Name", "Custom Backdrop").AsString).AsString;
                        Console.WriteLine($"[StarGiftAttributeGenerator] Selected Backdrop: {backdropName} (Rarity={selectedBackdrop.rarity}‰)");
                        
                        attributes.Add(new TStarGiftAttributeBackdrop
                        {
                            Name = backdropName,
                            BackdropId = backdropIdValue,
                            CenterColor = selectedBackdrop.backdrop.GetValue("CenterColor", 0x007AFF).ToInt32(),
                            EdgeColor = selectedBackdrop.backdrop.GetValue("EdgeColor", 0x0A84FF).ToInt32(),
                            PatternColor = selectedBackdrop.backdrop.GetValue("PatternColor", 0x5AC8FA).ToInt32(),
                            TextColor = selectedBackdrop.backdrop.GetValue("TextColor", 0xFFFFFF).ToInt32(),
                            RarityPermille = selectedBackdrop.rarity
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[StarGiftAttributeGenerator] Error loading upgrade attributes: {ex.Message}");
        }

        // Генерируем случайный backdrop из заранее заданного пула (только если он не был загружен из базы)
        var hasBackdrop = attributes.Any(a => a is TStarGiftAttributeBackdrop);
        if (!hasBackdrop)
        {
            var backdrop = GenerateRandomBackdrop(uniqueId);
            attributes.Add(backdrop);
        }

        var modelCount = attributes.Count(a => a is TStarGiftAttributeModel);
        var patternCount = attributes.Count(a => a is TStarGiftAttributePattern);
        var backdropCount = attributes.Count(a => a is TStarGiftAttributeBackdrop);

        Console.WriteLine($"[StarGiftAttributeGenerator] Generated {attributes.Count} unique upgrade attributes for gift {giftId}: {modelCount} model(s), {patternCount} pattern(s), {backdropCount} backdrop(s)");
        Console.WriteLine($"[StarGiftAttributeGenerator] Used base seed: {baseSeed}, Model seed: {baseSeed + 1}, Pattern seed: {baseSeed + 2}, Backdrop seed: {baseSeed + 3}");

        return attributes;
    }
    
    /// <summary>
    /// Выбирает элемент из списка с учётом вероятности RarityPermille.
    /// Чем выше rarity, тем РЕЖЕ элемент (меньше шанс выпадения).
    /// RarityPermille = 1000 означает 100% (всегда),
    /// RarityPermille = 500 - шанс 50%,
    /// RarityPermille = 100 - шанс 10% (редко),
    /// RarityPermille = 10 - шанс 1% (очень редко)
    /// </summary>
    private T SelectByRarity<T>(List<T> items, Random random)
    {
        if (items.Count == 0)
            return default;

        if (items.Count == 1)
            return items[0];

        // Извлекаем значения rarity более универсальным способом
        var rarities = new List<int>();
        var type = typeof(T);

        // Заранее определяем, как лучше получить rarity для данного типа.
        // 1. Проверяем свойства (анонимные типы)
        var rarityProp = type.GetProperty("Rarity", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase)
            ?? type.GetProperty("RarityPermille", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);

        // 2. Проверяем поля (именованные кортежи в рантайме - это Item1, Item2 и т.д.)
        var rarityField = type.GetField("rarity", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase)
            ?? type.GetField("rarityPermille", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);

        // 3. Отдельно проверяем Item3 (используется в кортежах апгрейда)
        var item3Field = (type.IsValueType && type.Name.StartsWith("ValueTuple")) ? type.GetField("Item3") : null;

        foreach (var item in items)
        {
            int rarity = 1000;
            if (rarityProp != null) rarity = (int)rarityProp.GetValue(item)!;
            else if (rarityField != null) rarity = (int)rarityField.GetValue(item)!;
            else if (item3Field != null) rarity = (int)item3Field.GetValue(item)!;
            
            rarities.Add(rarity);
        }
        
        // Считаем суммарный вес (сумму всех rarity)
        var totalWeight = rarities.Sum();

        if (totalWeight == 0)
        {
            // Если все rarity равны 0, выбираем случайно
            return items[random.Next(items.Count)];
        }

        // Генерируем случайное число от 0 до totalWeight
        var randomValue = random.Next(totalWeight);

        // Выбираем элемент по взвешенной вероятности
        var currentWeight = 0;
        for (int i = 0; i < items.Count; i++)
        {
            currentWeight += rarities[i];
            if (randomValue < currentWeight)
            {
                // Console.WriteLine($"[SelectByRarity] Selected item #{i + 1} with rarity {rarities[i]} (roll={randomValue}/{totalWeight})");
                return items[i];
            }
        }

        // Запасной вариант: последний элемент
        return items[items.Count - 1];
    }

    /// <summary>
    /// Генерирует набор атрибутов для превью с несколькими вариантами каждого типа.
    /// Клиент случайно комбинирует их, чтобы показать разнообразие
    /// </summary>
    public TVector<IStarGiftAttribute> GenerateSampleAttributesForPreview(long giftId)
    {
        var attributes = new TVector<IStarGiftAttribute>();

        try
        {
            // Загружаем ВСЕ наборы атрибутов для этого подарка из базы
            var collection = _mongoDatabase.GetCollection<BsonDocument>("StarGiftAttributeSets");
            var filter = Builders<BsonDocument>.Filter.Eq("GiftId", giftId);
            var attributeSets = collection.Find(filter).ToList();

            if (attributeSets.Count > 0)
            {
                Console.WriteLine($"[StarGiftAttributeGenerator] Found {attributeSets.Count} attribute sets for gift {giftId}");

                // Загружаем ВСЕ модели из ВСЕХ наборов атрибутов
                foreach (var attributeSet in attributeSets)
                {
                    if (attributeSet.Contains("Model") && !attributeSet["Model"].IsBsonNull)
                    {
                        var model = attributeSet["Model"].AsBsonDocument;
                        var docId = model.GetValue("DocumentId", 0L).ToInt64();
                        var document = LoadDocumentFromDb(docId);
                        
                        if (document != null)
                        {
                            attributes.Add(new TStarGiftAttributeModel
                            {
                                Name = attributeSet.GetValue("ModelName", "Model").AsString,
                                Document = document,
                                RarityPermille = model.GetValue("RarityPermille", 1000).ToInt32()
                            });
                        }
                    }
                }
                
                // Загружаем ВСЕ паттерны из ВСЕХ наборов атрибутов
                foreach (var attributeSet in attributeSets)
                {
                    if (attributeSet.Contains("Pattern") && !attributeSet["Pattern"].IsBsonNull)
                    {
                        var pattern = attributeSet["Pattern"].AsBsonDocument;
                        var docId = pattern.GetValue("DocumentId", 0L).ToInt64();
                        var document = LoadDocumentFromDb(docId);
                        
                        if (document != null)
                        {
                            attributes.Add(new TStarGiftAttributePattern
                            {
                                Name = attributeSet.GetValue("PatternName", "Pattern").AsString,
                                Document = document,
                                RarityPermille = pattern.GetValue("RarityPermille", 1000).ToInt32()
                            });
                        }
                    }
                }
                
                // Генерируем 30 случайных фонов для анимации превью (для большего разнообразия)
                for (int i = 0; i < 30; i++)
                {
                    var randomId = Guid.NewGuid().ToString();
                    attributes.Add(GenerateRandomBackdrop(randomId));
                }

                var modelCount = attributes.Count(a => a is TStarGiftAttributeModel);
                var patternCount = attributes.Count(a => a is TStarGiftAttributePattern);
                var backdropCount = attributes.Count(a => a is TStarGiftAttributeBackdrop);
                Console.WriteLine($"[StarGiftAttributeGenerator] Generated {attributes.Count} preview samples ({modelCount} models, {patternCount} patterns, {backdropCount} backdrops)");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[StarGiftAttributeGenerator] Error generating preview samples: {ex.Message}");
        }

        // Если ничего не загрузилось, возвращаем заглушку
        if (attributes.Count == 0)
        {
            return GeneratePlaceholderAttributes(giftId);
        }
        
        return attributes;
    }

    /// <summary>
    /// Генерирует случайный фон из пула в 80 вариантов (официальные фоны Telegram)
    /// </summary>
    private TStarGiftAttributeBackdrop GenerateRandomBackdrop(string uniqueId)
    {
        // Пул из 80 официальных цветовых схем фонов Telegram (взяты из реального Telegram API)
        var backdrops = new[]
        {
            new { Name = "Lemongrass", CenterColor = 0xAEA5DA, EdgeColor = 0x559945, PatternColor = 0x466A97, TextColor = 0xD91042, Rarity = 10 },
            new { Name = "Indigo Dye", CenterColor = 0x537A91, EdgeColor = 0x416189, PatternColor = 0x031B29, TextColor = 0xC2DCEE, Rarity = 15 },
            new { Name = "Camo Green", CenterColor = 0x7598CD, EdgeColor = 0x546C21, PatternColor = 0x163501, TextColor = 0xCFC329, Rarity = 15 },
            new { Name = "Old Gold", CenterColor = 0xB58DB8, EdgeColor = 0x946295, PatternColor = 0x4F2782, TextColor = 0xFFAC9C, Rarity = 10 },
            new { Name = "Mustard", CenterColor = 0xD49B8D, EdgeColor = 0xC47F92, PatternColor = 0x7A2000, TextColor = 0xFFB9AC, Rarity = 15 },
            new { Name = "Lavender", CenterColor = 0xB77FE4, EdgeColor = 0x8A644C, PatternColor = 0x5B0EBB, TextColor = 0xE8C0FF, Rarity = 15 },
            new { Name = "Sky Blue", CenterColor = 0x58AE48, EdgeColor = 0x538342, PatternColor = 0x07613B, TextColor = 0xCE0A5D, Rarity = 10 },
            new { Name = "Coral Red", CenterColor = 0xDA72EB, EdgeColor = 0xC462CF, PatternColor = 0x890000, TextColor = 0xFFAF52, Rarity = 10 },
            new { Name = "Gunmetal", CenterColor = 0x4C5163, EdgeColor = 0x2F3BC2, PatternColor = 0x04081A, TextColor = 0xB6C3DC, Rarity = 15 },
            new { Name = "Caramel", CenterColor = 0xD0C3B2, EdgeColor = 0xB790C1, PatternColor = 0x7D4000, TextColor = 0xFFAED3, Rarity = 12 },
            new { Name = "Pistachio", CenterColor = 0x97947C, EdgeColor = 0x5C876C, PatternColor = 0x28492B, TextColor = 0xDA1AC9, Rarity = 12 },
            new { Name = "Turquoise", CenterColor = 0x5EB738, EdgeColor = 0x3D8E0E, PatternColor = 0x11523C, TextColor = 0xBDFBF2, Rarity = 10 },
            new { Name = "Chestnut", CenterColor = 0xBE79D4, EdgeColor = 0x994AB8, PatternColor = 0x601988, TextColor = 0xFEA949, Rarity = 12 },
            new { Name = "Chocolate", CenterColor = 0xA476D8, EdgeColor = 0x745EAB, PatternColor = 0x3E1802, TextColor = 0xE4D12C, Rarity = 12 },
            new { Name = "Neon Blue", CenterColor = 0x7597E9, EdgeColor = 0x685CE4, PatternColor = 0x28283C, TextColor = 0xCFD87F, Rarity = 12 },
            new { Name = "Aquamarine", CenterColor = 0x60B115, EdgeColor = 0x46A9B4, PatternColor = 0x035FD7, TextColor = 0xC7F77E, Rarity = 12 },
            new { Name = "Platinum", CenterColor = 0xB2A427, EdgeColor = 0x887EFE, PatternColor = 0x3D378D, TextColor = 0xE9F462, Rarity = 15 },
            new { Name = "Steel Grey", CenterColor = 0x97A13C, EdgeColor = 0x637A7C, PatternColor = 0x334DD2, TextColor = 0xDFF8E8, Rarity = 12 },
            new { Name = "Fire Engine", CenterColor = 0xF042CF, EdgeColor = 0xC42649, PatternColor = 0x6900F9, TextColor = 0xFFB1A6, Rarity = 12 },
            new { Name = "Azure Blue", CenterColor = 0x5DB5CB, EdgeColor = 0x448EAB, PatternColor = 0x025014, TextColor = 0xB5E7FF, Rarity = 15 },
            new { Name = "Orange", CenterColor = 0xD1805A, EdgeColor = 0xC07857, PatternColor = 0x9D3787, TextColor = 0xFFBD43, Rarity = 10 },
            new { Name = "Persimmon", CenterColor = 0xE7ACDA, EdgeColor = 0xC56ADF, PatternColor = 0xAD1600, TextColor = 0xFFBB57, Rarity = 12 },
            new { Name = "Khaki Green", CenterColor = 0xADB470, EdgeColor = 0x6B75D4, PatternColor = 0x39521B, TextColor = 0xD4014B, Rarity = 10 },
            new { Name = "French Blue", CenterColor = 0x5CA144, EdgeColor = 0x376E0A, PatternColor = 0x073ACC, TextColor = 0xC1F319, Rarity = 15 },
            new { Name = "Navy Blue", CenterColor = 0x6CBC5D, EdgeColor = 0x5C7B49, PatternColor = 0x123A22, TextColor = 0xD3FA7F, Rarity = 12 },
            new { Name = "Raspberry", CenterColor = 0xE08485, EdgeColor = 0xB65800, PatternColor = 0x890038, TextColor = 0xFFB366, Rarity = 12 },
            new { Name = "Roman Silver", CenterColor = 0xA3C535, EdgeColor = 0x7C7F0A, PatternColor = 0x3F42D0, TextColor = 0xDAF5A2, Rarity = 10 },
            new { Name = "Grape", CenterColor = 0x9D5B41, EdgeColor = 0x795520, PatternColor = 0x3E0EEB, TextColor = 0xE0E2FE, Rarity = 12 },
            new { Name = "Deep Cyan", CenterColor = 0x31B02A, EdgeColor = 0x189029, PatternColor = 0x004F4F, TextColor = 0xD1F8FD, Rarity = 10 },
            new { Name = "Fandango", CenterColor = 0xE27B36, EdgeColor = 0xA4709B, PatternColor = 0x8E174E, TextColor = 0xFFB5DB, Rarity = 15 },
            new { Name = "Dark Lilac", CenterColor = 0xB18125, EdgeColor = 0x8C57FA, PatternColor = 0x652C52, TextColor = 0xF0B9E2, Rarity = 10 },
            new { Name = "Feldgrau", CenterColor = 0x899988, EdgeColor = 0x5E6F63, PatternColor = 0x1C2A1F, TextColor = 0xDEBC61, Rarity = 12 },
            new { Name = "Pacific Cyan", CenterColor = 0x5AC626, EdgeColor = 0x3D8F2A, PatternColor = 0x02650D, TextColor = 0xB6EB7F, Rarity = 12 },
            new { Name = "Celtic Blue", CenterColor = 0x45B66D, EdgeColor = 0x388479, PatternColor = 0x003E85, TextColor = 0xC2C4FF, Rarity = 10 },
            new { Name = "Cyberpunk", CenterColor = 0x859CF3, EdgeColor = 0x866EF3, PatternColor = 0x431936, TextColor = 0xE1097F, Rarity = 12 },
            new { Name = "Pacific Green", CenterColor = 0x6FAD93, EdgeColor = 0x3BA084, PatternColor = 0x006149, TextColor = 0xC6E770, Rarity = 15 },
            new { Name = "Strawberry", CenterColor = 0xDD79EF, EdgeColor = 0xB76460, PatternColor = 0xA93E8C, TextColor = 0xFFB5C3, Rarity = 15 },
            new { Name = "Sapphire", CenterColor = 0x589AC8, EdgeColor = 0x5370C2, PatternColor = 0x0D43B6, TextColor = 0xC1E0FF, Rarity = 10 },
            new { Name = "Rosewood", CenterColor = 0xB75267, EdgeColor = 0x814AD2, PatternColor = 0x551222, TextColor = 0xEDD3CD, Rarity = 15 },
            new { Name = "Ivory White", CenterColor = 0xBADEB1, EdgeColor = 0xA1AAA7, PatternColor = 0x665C52, TextColor = 0xF5F332, Rarity = 12 },
            new { Name = "Mystic Pearl", CenterColor = 0xD0BC6D, EdgeColor = 0xB063F0, PatternColor = 0x9AF026, TextColor = 0xFEE5E0, Rarity = 10 },
            new { Name = "English Violet", CenterColor = 0xB18BAB, EdgeColor = 0x876491, PatternColor = 0x542ADF, TextColor = 0xE6C3ED, Rarity = 10 },
            new { Name = "Moonstone", CenterColor = 0x7EC734, EdgeColor = 0x5880B0, PatternColor = 0x164152, TextColor = 0xDAEBDD, Rarity = 15 },
            new { Name = "French Violet", CenterColor = 0xC28166, EdgeColor = 0x9152E9, PatternColor = 0x4A038A, TextColor = 0xEBE1FF, Rarity = 10 },
            new { Name = "Emerald", CenterColor = 0x789A05, EdgeColor = 0x429FF1, PatternColor = 0x006522, TextColor = 0xB9DDC9, Rarity = 15 },
            new { Name = "Carrot Juice", CenterColor = 0xDBB3E7, EdgeColor = 0xC78ECF, PatternColor = 0x8E2780, TextColor = 0xFFBA4A, Rarity = 10 },
            new { Name = "Hunter Green", CenterColor = 0x8FB0F8, EdgeColor = 0x4B825B, PatternColor = 0x1C4A1F, TextColor = 0xD8DE5E, Rarity = 12 },
            new { Name = "Desert Sand", CenterColor = 0xB3C102, EdgeColor = 0x7E78DB, PatternColor = 0x504A19, TextColor = 0xF2F7CD, Rarity = 15 },
            new { Name = "Gunship Green", CenterColor = 0x559C75, EdgeColor = 0x3D6857, PatternColor = 0x07291D, TextColor = 0xB5D654, Rarity = 15 },
            new { Name = "Battleship Grey", CenterColor = 0x8C9285, EdgeColor = 0x6C68E6, PatternColor = 0x2B28A0, TextColor = 0xCFDB64, Rarity = 15 },
            new { Name = "Mint Green", CenterColor = 0x7EC082, EdgeColor = 0x45994A, PatternColor = 0x026AA2, TextColor = 0xBDC4DC, Rarity = 10 },
            new { Name = "Midnight Blue", CenterColor = 0x5C7A05, EdgeColor = 0x353FD7, PatternColor = 0x030A18, TextColor = 0xBFC260, Rarity = 12 },
            new { Name = "Burnt Sienna", CenterColor = 0xD64C3C, EdgeColor = 0xB52CAD, PatternColor = 0x6B1E02, TextColor = 0xFFB330, Rarity = 12 },
            new { Name = "Carmine", CenterColor = 0xE09CCA, EdgeColor = 0xA7EDCB, PatternColor = 0x4F0800, TextColor = 0xFF9CCD, Rarity = 10 },
            new { Name = "Rifle Green", CenterColor = 0x64665C, EdgeColor = 0x4B5EC1, PatternColor = 0x0F0F0B, TextColor = 0xC3A7DD, Rarity = 15 },
            new { Name = "Copper", CenterColor = 0xD09656, EdgeColor = 0x9D59B1, PatternColor = 0x602E01, TextColor = 0xF4DCDE, Rarity = 12 },
            new { Name = "Purple", CenterColor = 0xAE5FAE, EdgeColor = 0x842384, PatternColor = 0x470C47, TextColor = 0xF3DA73, Rarity = 12 },
            new { Name = "Jade Green", CenterColor = 0x55C89C, EdgeColor = 0x3B9EE7, PatternColor = 0x044931, TextColor = 0xBEF8D7, Rarity = 12 },
            new { Name = "Satin Gold", CenterColor = 0xBF9247, EdgeColor = 0x8D9AB9, PatternColor = 0x5D3C00, TextColor = 0xFED5B9, Rarity = 15 },
            new { Name = "Pine Green", CenterColor = 0x6BB87C, EdgeColor = 0x3E70F0, PatternColor = 0x0B4733, TextColor = 0xD8DEA5, Rarity = 10 },
            new { Name = "Burgundy", CenterColor = 0xA35466, EdgeColor = 0x6D4A5A, PatternColor = 0x340117, TextColor = 0xE7BE40, Rarity = 10 },
            new { Name = "Pure Gold", CenterColor = 0xCCAE41, EdgeColor = 0x986C32, PatternColor = 0x703B80, TextColor = 0xFFCB2B, Rarity = 12 },
            new { Name = "Electric Purple", CenterColor = 0xCA5AC6, EdgeColor = 0x966754, PatternColor = 0x621134, TextColor = 0xEBCBFF, Rarity = 12 },
            new { Name = "Amber", CenterColor = 0xDAB2C5, EdgeColor = 0xB1772A, PatternColor = 0x7A2180, TextColor = 0xFFD6D7, Rarity = 10 },
            new { Name = "Marine Blue", CenterColor = 0x4E6E1C, EdgeColor = 0x3B406A, PatternColor = 0x010821, TextColor = 0xBBEB52, Rarity = 10 },
            new { Name = "Electric Indigo", CenterColor = 0xA991F3, EdgeColor = 0x5B5CD8, PatternColor = 0x37282B, TextColor = 0xD8E3FF, Rarity = 15 },
            new { Name = "Seal Brown", CenterColor = 0x665155, EdgeColor = 0x4737AE, PatternColor = 0x0A0515, TextColor = 0xD4E8B5, Rarity = 12 },
            new { Name = "Silver Blue", CenterColor = 0x80AC48, EdgeColor = 0x607C11, PatternColor = 0x153A4B, TextColor = 0xC9C174, Rarity = 15 },
            new { Name = "Dark Green", CenterColor = 0x515CC1, EdgeColor = 0x2B442F, PatternColor = 0x000501, TextColor = 0xBFC7A9, Rarity = 10 },
            new { Name = "Black", CenterColor = 0x363538, EdgeColor = 0x0E0F0F, PatternColor = 0x6C6068, TextColor = 0x8C6271, Rarity = 10 },
            new { Name = "Ranger Green", CenterColor = 0x5F73C9, EdgeColor = 0x3C4F3B, PatternColor = 0x101F19, TextColor = 0xB7D2B5, Rarity = 15 },
            new { Name = "Cappuccino", CenterColor = 0xB17BFE, EdgeColor = 0x7C74D6, PatternColor = 0x4A3126, TextColor = 0xEBEFC8, Rarity = 10 },
            new { Name = "Tactical Pine", CenterColor = 0x4480DB, EdgeColor = 0x2F5D69, PatternColor = 0x002624, TextColor = 0xB7E553, Rarity = 15 },
            new { Name = "Onyx Black", CenterColor = 0x4D54D4, EdgeColor = 0x313548, PatternColor = 0x000000, TextColor = 0xA9C62D, Rarity = 12 },
            new { Name = "Cobalt Blue", CenterColor = 0x6088CF, EdgeColor = 0x515B38, PatternColor = 0x1320FC, TextColor = 0xC2A3F5, Rarity = 12 },
            new { Name = "Tomato", CenterColor = 0xE65F3E, EdgeColor = 0xD42BBF, PatternColor = 0x800800, TextColor = 0xFFB2BD, Rarity = 15 },
            new { Name = "Malachite", CenterColor = 0x95AE57, EdgeColor = 0x3D8EE5, PatternColor = 0x046B06, TextColor = 0xC2E4DE, Rarity = 10 },
            new { Name = "Mexican Pink", CenterColor = 0xE35A92, EdgeColor = 0xC927EC, PatternColor = 0x74FB30, TextColor = 0xFFB366, Rarity = 15 },
            new { Name = "Shamrock Green", CenterColor = 0x8A9163, EdgeColor = 0x559945, PatternColor = 0x126B80, TextColor = 0xD5EB38, Rarity = 12 },
            new { Name = "Light Olive", CenterColor = 0xC2B864, EdgeColor = 0x889845, PatternColor = 0x594E04, TextColor = 0xF5F63C, Rarity = 12 }
        };
        
        // По хэшу uniqueId детерминированно выбираем фон (одному подарку всегда соответствует один фон)
        var hash = Math.Abs(uniqueId.GetHashCode());
        var index = hash % backdrops.Length;
        var selected = backdrops[index];
        
        return new TStarGiftAttributeBackdrop
        {
            Name = selected.Name,
            BackdropId = hash,
            CenterColor = selected.CenterColor,
            EdgeColor = selected.EdgeColor,
            PatternColor = selected.PatternColor,
            TextColor = selected.TextColor,
            RarityPermille = selected.Rarity
        };
    }
}
