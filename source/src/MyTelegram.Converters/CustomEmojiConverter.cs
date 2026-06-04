using MyTelegram.ReadModel.Interfaces;
using MyTelegram.Schema;

namespace MyTelegram.Converters;

/// <summary>
/// Конвертер для преобразования ReadModel в TL-схему
/// </summary>
public static class CustomEmojiConverter
{
    /// <summary>
    /// Конвертировать StickerSet в messages.stickerSet для custom emoji
    /// </summary>
    public static MyTelegram.Schema.Messages.IStickerSet ToTlStickerSet(
        IStickerSetReadModel stickerSet,
        List<ICustomEmojiReadModel> documents,
        List<IEmojiPackReadModel> packs,
        List<IEmojiKeywordReadModel> keywords)
    {
        // Создаем TL StickerSet
        var tlSet = new TStickerSet
        {
            Id = stickerSet.StickerSetId,
            AccessHash = stickerSet.AccessHash,
            Title = stickerSet.Title,
            ShortName = stickerSet.ShortName,
            Count = stickerSet.Count,
            Emojis = stickerSet.Emojis,
            TextColor = stickerSet.TextColor,
            ChannelEmojiStatus = stickerSet.ChannelEmojiStatus,
            Masks = stickerSet.Masks,
            Archived = false,
            Official = false
        };

        // Thumbnails
        if (stickerSet.Thumbs != null && stickerSet.Thumbs.Count > 0)
        {
            tlSet.Thumbs = new TVector<MyTelegram.Schema.IPhotoSize>(
                stickerSet.Thumbs.Select(ConvertPhotoSize).ToList());
        }

        if (stickerSet.ThumbDocumentId.HasValue)
        {
            tlSet.ThumbDocumentId = stickerSet.ThumbDocumentId.Value;
        }

        // Конвертируем packs
        var tlPacks = packs.Select(p => new TStickerPack
        {
            Emoticon = p.Emoticon,
            Documents = new TVector<long>(p.DocumentIds)
        }).Cast<MyTelegram.Schema.IStickerPack>().ToList();

        // Конвертируем keywords
        var tlKeywords = keywords.Select(k => new TStickerKeyword
        {
            DocumentId = k.DocumentId,
            Keyword = new TVector<string>(k.Keywords)
        }).Cast<MyTelegram.Schema.IStickerKeyword>().ToList();

        // Конвертируем documents
        var tlDocuments = documents.Select(ConvertToTlDocument)
            .Cast<MyTelegram.Schema.IDocument>()
            .ToList();

        return new MyTelegram.Schema.Messages.TStickerSet
        {
            Set = tlSet,
            Packs = new TVector<MyTelegram.Schema.IStickerPack>(tlPacks),
            Keywords = new TVector<MyTelegram.Schema.IStickerKeyword>(tlKeywords),
            Documents = new TVector<MyTelegram.Schema.IDocument>(tlDocuments)
        };
    }

    /// <summary>
    /// Конвертировать CustomEmoji в TL Document
    /// </summary>
    public static TDocument ConvertToTlDocument(ICustomEmojiReadModel emoji)
    {
        // Создаем documentAttributeCustomEmoji
        var customEmojiAttr = new TDocumentAttributeCustomEmoji
        {
            Free = emoji.IsFree,
            TextColor = emoji.HasTextColor,
            Alt = emoji.Alt,
            Stickerset = new TInputStickerSetID
            {
                Id = emoji.StickerSetId,
                AccessHash = 0 // Заполняется из стикерсета
            }
        };

        var document = new TDocument
        {
            Id = emoji.DocumentId,
            AccessHash = emoji.AccessHash,
            FileReference = emoji.FileReference.IsEmpty ? Array.Empty<byte>() : emoji.FileReference.ToArray(),
            Date = emoji.Date,
            MimeType = emoji.MimeType ?? "application/x-tgsticker",
            Size = emoji.Size,
            DcId = emoji.DcId,
            Attributes = new TVector<IDocumentAttribute>(new List<IDocumentAttribute> { customEmojiAttr })
        };

        // Thumbnails
        if (emoji.Thumbs != null && emoji.Thumbs.Count > 0)
        {
            document.Thumbs = new TVector<MyTelegram.Schema.IPhotoSize>(
                emoji.Thumbs.Select(ConvertPhotoSize).ToList());
        }

        if (emoji.VideoThumbs != null && emoji.VideoThumbs.Count > 0)
        {
            document.VideoThumbs = new TVector<MyTelegram.Schema.IVideoSize>(
                emoji.VideoThumbs.Select(ConvertVideoSize).ToList());
        }

        return document;
    }

    /// <summary>
    /// Конвертировать DocumentReadModel в TL Document для sticker set
    /// </summary>
    public static TDocument ConvertDocumentToTl(IDocumentReadModel doc, IStickerSetReadModel stickerSet, string? emoji = null)
    {
        // Используем существующие атрибуты из документа, если они есть
        List<IDocumentAttribute> attributes;
        
        if (doc.Attributes2 != null && doc.Attributes2.Count > 0)
        {
            // Документ уже имеет правильные атрибуты из БД
            // ВАЖНО: Проверяем и исправляем TDocumentAttributeSticker если нужно
            attributes = new List<IDocumentAttribute>();
            foreach (var attr in doc.Attributes2)
            {
                if (attr is TDocumentAttributeSticker stickerAttr)
                {
                    // Если stickerSet передан, используем его (логика ниже/выше, но здесь мы просто сохраняем атрибут)
                    // Если stickerSet == null, мы должны сохранить то, что есть в БД, если оно валидное.
                    // Force Empty ONLY if both are null/invalid.
                    
                    if (stickerSet == null && stickerAttr.Stickerset == null)
                    {
                        attributes.Add(new TDocumentAttributeSticker
                        {
                            Alt = stickerAttr.Alt ?? emoji ?? "🎁",
                            Stickerset = new TInputStickerSetEmpty(),
                            Mask = stickerAttr.Mask,
                            MaskCoords = stickerAttr.MaskCoords
                        });
                    }
                    else
                    {
                        // Keep existing attribute (which might have TInputStickerSetShortName from gifts.js)
                        attributes.Add(attr);
                    }
                }
                else if (attr is TDocumentAttributeAnimated && stickerSet == null)
                {
                    // SKIP TDocumentAttributeAnimated for gift stickers!
                    // tdesktop processes attributes in order and AnimatedDocument type
                    // prevents StickerDocument type from being set correctly
                    continue;
                }
                else
                {
                    attributes.Add(attr);
                }
            }
            
            // Убедимся что есть хотя бы один TDocumentAttributeSticker
            if (!attributes.Any(a => a is TDocumentAttributeSticker))
            {
                attributes.Add(new TDocumentAttributeSticker
                {
                    Alt = emoji ?? "🎁",
                    Stickerset = new TInputStickerSetID { Id = 7770001, AccessHash = 7770001 }
                });
            }
        }
        else if (stickerSet != null)
        {
            // Создаем атрибуты на лету если есть stickerSet
            IDocumentAttribute attribute;
            
            if (stickerSet.Emojis)
            {
                // Для Custom Emoji
                attribute = new TDocumentAttributeCustomEmoji
                {
                    Free = true, // По умолчанию бесплатный
                    TextColor = stickerSet.TextColor,
                    Alt = emoji ?? "🙂",
                    Stickerset = new TInputStickerSetID
                    {
                        Id = stickerSet.StickerSetId,
                        AccessHash = stickerSet.AccessHash
                    }
                };
            }
            else
            {
                // Для обычных стикеров
                attribute = new TDocumentAttributeSticker
                {
                    Mask = stickerSet.Masks,
                    Alt = emoji ?? "🙂",
                    Stickerset = new TInputStickerSetID
                    {
                        Id = stickerSet.StickerSetId,
                        AccessHash = stickerSet.AccessHash
                    }
                };
            }
            
            attributes = new List<IDocumentAttribute> { attribute };
        }
        else
        {
            // Нет ни атрибутов, ни stickerSet - создаем базовый TDocumentAttributeSticker
            attributes = new List<IDocumentAttribute>
            {
                new TDocumentAttributeSticker
                {
                    Alt = emoji ?? "🎁",
                    Stickerset = new TInputStickerSetID { Id = 7770001, AccessHash = 7770001 }
                }
            };
        }

        var document = new TDocument
        {
            Id = doc.DocumentId,
            AccessHash = doc.AccessHash,
            FileReference = doc.FileReference.IsEmpty ? Array.Empty<byte>() : doc.FileReference.ToArray(),
            Date = doc.Date,
            MimeType = doc.MimeType ?? "application/x-tgsticker",
            Size = doc.Size,
            DcId = doc.DcId,
            Attributes = new TVector<IDocumentAttribute>(attributes)
        };

        // Thumbnails
        if (doc.Thumbs != null && doc.Thumbs.Count > 0)
        {
            document.Thumbs = new TVector<MyTelegram.Schema.IPhotoSize>(
                doc.Thumbs.Select(ConvertPhotoSize).ToList());
        }

        if (doc.VideoThumbs != null && doc.VideoThumbs.Count > 0)
        {
            document.VideoThumbs = new TVector<MyTelegram.Schema.IVideoSize>(
                doc.VideoThumbs.Select(ConvertVideoSize).ToList());
        }

        return document;
    }

    /// <summary>
    /// Конвертировать EmojiGroups в TL
    /// </summary>
    public static MyTelegram.Schema.Messages.IEmojiGroups ToTlEmojiGroups(
        IEmojiGroupReadModel? groupReadModel)
    {
        if (groupReadModel == null)
        {
            return new MyTelegram.Schema.Messages.TEmojiGroupsNotModified();
        }

        var tlGroups = groupReadModel.Groups.Select(g =>
        {
            if (g.IsPremium)
            {
                return (MyTelegram.Schema.IEmojiGroup)new TEmojiGroupPremium
                {
                    Title = g.Title,
                    IconEmojiId = g.IconEmojiId
                };
            }

            if (g.GroupType == "greeting")
            {
                return (MyTelegram.Schema.IEmojiGroup)new TEmojiGroupGreeting
                {
                    Title = g.Title,
                    IconEmojiId = g.IconEmojiId,
                    Emoticons = new TVector<string>(g.Emoticons)
                };
            }

            return (MyTelegram.Schema.IEmojiGroup)new TEmojiGroup
            {
                Title = g.Title,
                IconEmojiId = g.IconEmojiId,
                Emoticons = new TVector<string>(g.Emoticons)
            };
        }).ToList();

        return new MyTelegram.Schema.Messages.TEmojiGroups
        {
            Hash = (int)groupReadModel.Hash,
            Groups = new TVector<MyTelegram.Schema.IEmojiGroup>(tlGroups)
        };
    }

    private static IPhotoSize ConvertPhotoSize(PhotoSize size)
    {
        switch (size.Type)
        {
            case "i":
                return new TPhotoStrippedSize { Type = size.Type, Bytes = size.Bytes };
            case "j":
                return new TPhotoPathSize { Type = size.Type, Bytes = size.Bytes };
            default:
                return new TPhotoSize
                {
                    Type = size.Type,
                    W = size.W,
                    H = size.H,
                    Size = (int)size.Size
                };
        }
    }

    private static IVideoSize ConvertVideoSize(VideoSize size)
    {
        return new TVideoSize
        {
            Type = size.Type,
            W = size.W,
            H = size.H,
            Size = (int)size.Size
        };
    }
}
