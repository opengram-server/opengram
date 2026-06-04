using MyTelegram.Domain.Shared.BotApi;
using MyTelegram.Schema;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MyTelegram.BotApi.Helpers;

/// <summary>
/// Helper methods for Bot API
/// </summary>
public static class BotApiHelpers
{
    /// <summary>
    /// Parse parse_mode and convert text to entities
    /// </summary>
    public static List<IMessageEntity>? ParseEntities(string? text, string? parseMode, List<IMessageEntity>? existingEntities = null)
    {
        if (string.IsNullOrEmpty(text))
        {
            return existingEntities;
        }

        var entities = existingEntities?.ToList() ?? new List<IMessageEntity>();

        if (string.IsNullOrEmpty(parseMode))
        {
            return entities.Count > 0 ? entities : null;
        }

        switch (parseMode.ToLowerInvariant())
        {
            case "markdown":
                ParseMarkdown(text, entities);
                break;
            case "markdownv2":
                ParseMarkdownV2(text, entities);
                break;
            case "html":
                ParseHtml(text, entities);
                break;
        }

        return entities.Count > 0 ? entities : null;
    }

    private static void ParseMarkdown(string text, List<IMessageEntity> entities)
    {
        // Simple Markdown parser for basic formatting
        // *bold* _italic_ `code` ```pre``` [text](url)
        
        // Bold: *text*
        AddEntitiesFromPattern(text, @"\*([^\*]+)\*", entities, () => new TMessageEntityBold());
        
        // Italic: _text_
        AddEntitiesFromPattern(text, @"_([^_]+)_", entities, () => new TMessageEntityItalic());
        
        // Code: `text`
        AddEntitiesFromPattern(text, @"`([^`]+)`", entities, () => new TMessageEntityCode());
        
        // Pre: ```text```
        AddEntitiesFromPattern(text, @"```([^`]+)```", entities, () => new TMessageEntityPre());
        
        // Links: [text](url)
        var linkPattern = @"\[([^\]]+)\]\(([^\)]+)\)";
        var matches = Regex.Matches(text, linkPattern);
        foreach (Match match in matches)
        {
            entities.Add(new TMessageEntityTextUrl
            {
                Offset = match.Index,
                Length = match.Groups[1].Length,
                Url = match.Groups[2].Value
            });
        }
    }

    private static void ParseMarkdownV2(string text, List<IMessageEntity> entities)
    {
        // MarkdownV2 with escaping support
        // Similar to Markdown but with more features
        ParseMarkdown(text, entities); // Simplified - full implementation would handle escaping
    }

    private static void ParseHtml(string text, List<IMessageEntity> entities)
    {
        // HTML parser for basic tags
        // <b>bold</b> <i>italic</i> <code>code</code> <pre>pre</pre> <a href="url">text</a>
        
        AddEntitiesFromHtmlTag(text, "b", entities, () => new TMessageEntityBold());
        AddEntitiesFromHtmlTag(text, "strong", entities, () => new TMessageEntityBold());
        AddEntitiesFromHtmlTag(text, "i", entities, () => new TMessageEntityItalic());
        AddEntitiesFromHtmlTag(text, "em", entities, () => new TMessageEntityItalic());
        AddEntitiesFromHtmlTag(text, "u", entities, () => new TMessageEntityUnderline());
        AddEntitiesFromHtmlTag(text, "s", entities, () => new TMessageEntityStrike());
        AddEntitiesFromHtmlTag(text, "code", entities, () => new TMessageEntityCode());
        AddEntitiesFromHtmlTag(text, "pre", entities, () => new TMessageEntityPre());
        
        // Links: <a href="url">text</a>
        var linkPattern = @"<a\s+href=[""']([^""']+)[""']>([^<]+)</a>";
        var matches = Regex.Matches(text, linkPattern, RegexOptions.IgnoreCase);
        foreach (Match match in matches)
        {
            entities.Add(new TMessageEntityTextUrl
            {
                Offset = match.Index,
                Length = match.Groups[2].Length,
                Url = match.Groups[1].Value
            });
        }
    }

    private static void AddEntitiesFromPattern(string text, string pattern, List<IMessageEntity> entities, Func<IMessageEntity> entityFactory)
    {
        var matches = Regex.Matches(text, pattern);
        foreach (Match match in matches)
        {
            var entity = entityFactory();
            entity.Offset = match.Index;
            entity.Length = match.Groups[1].Length;
            entities.Add(entity);
        }
    }

    private static void AddEntitiesFromHtmlTag(string text, string tag, List<IMessageEntity> entities, Func<IMessageEntity> entityFactory)
    {
        var pattern = $@"<{tag}>([^<]+)</{tag}>";
        AddEntitiesFromPattern(text, pattern, entities, entityFactory);
    }

    /// <summary>
    /// Parse reply_markup from JSON
    /// </summary>
    public static IReplyMarkup? ParseReplyMarkup(JsonElement? replyMarkupJson)
    {
        if (!replyMarkupJson.HasValue || replyMarkupJson.Value.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        var json = replyMarkupJson.Value;

        // Check if it's InlineKeyboardMarkup
        if (json.TryGetProperty("inline_keyboard", out var inlineKeyboard))
        {
            var rows = new List<IKeyboardButtonRow>();
            
            foreach (var row in inlineKeyboard.EnumerateArray())
            {
                var buttons = new List<IKeyboardButton>();
                
                foreach (var button in row.EnumerateArray())
                {
                    var text = button.GetProperty("text").GetString() ?? "";
                    
                    IKeyboardButton keyboardButton;
                    
                    if (button.TryGetProperty("callback_data", out var callbackData))
                    {
                        keyboardButton = new TKeyboardButtonCallback
                        {
                            Text = text,
                            Data = callbackData.GetString() ?? ""
                        };
                    }
                    else if (button.TryGetProperty("url", out var url))
                    {
                        keyboardButton = new TKeyboardButtonUrl
                        {
                            Text = text,
                            Url = url.GetString() ?? ""
                        };
                    }
                    else if (button.TryGetProperty("switch_inline_query", out var switchInlineQuery))
                    {
                        keyboardButton = new TKeyboardButtonSwitchInline
                        {
                            Text = text,
                            Query = switchInlineQuery.GetString() ?? ""
                        };
                    }
                    else
                    {
                        // Default button
                        keyboardButton = new TKeyboardButton { Text = text };
                    }
                    
                    buttons.Add(keyboardButton);
                }
                
                rows.Add(new TKeyboardButtonRow
                {
                    Buttons = new TVector<IKeyboardButton>(buttons)
                });
            }
            
            return new TReplyInlineMarkup
            {
                Rows = new TVector<IKeyboardButtonRow>(rows)
            };
        }
        
        // Check if it's ReplyKeyboardMarkup
        if (json.TryGetProperty("keyboard", out var keyboard))
        {
            var rows = new List<IKeyboardButtonRow>();
            
            foreach (var row in keyboard.EnumerateArray())
            {
                var buttons = new List<IKeyboardButton>();
                
                foreach (var button in row.EnumerateArray())
                {
                    string text;
                    if (button.ValueKind == JsonValueKind.String)
                    {
                        text = button.GetString() ?? "";
                    }
                    else
                    {
                        text = button.GetProperty("text").GetString() ?? "";
                    }
                    
                    IKeyboardButton keyboardButton;
                    
                    if (button.ValueKind == JsonValueKind.Object && button.TryGetProperty("request_contact", out var requestContact) && requestContact.GetBoolean())
                    {
                        keyboardButton = new TKeyboardButtonRequestPhone { Text = text };
                    }
                    else if (button.ValueKind == JsonValueKind.Object && button.TryGetProperty("request_location", out var requestLocation) && requestLocation.GetBoolean())
                    {
                        keyboardButton = new TKeyboardButtonRequestGeoLocation { Text = text };
                    }
                    else
                    {
                        keyboardButton = new TKeyboardButton { Text = text };
                    }
                    
                    buttons.Add(keyboardButton);
                }
                
                rows.Add(new TKeyboardButtonRow
                {
                    Buttons = new TVector<IKeyboardButton>(buttons)
                });
            }
            
            var resizeKeyboard = json.TryGetProperty("resize_keyboard", out var resize) && resize.GetBoolean();
            var oneTimeKeyboard = json.TryGetProperty("one_time_keyboard", out var oneTime) && oneTime.GetBoolean();
            var selective = json.TryGetProperty("selective", out var sel) && sel.GetBoolean();
            
            return new TReplyKeyboardMarkup
            {
                Resize = resizeKeyboard,
                SingleUse = oneTimeKeyboard,
                Selective = selective,
                Rows = new TVector<IKeyboardButtonRow>(rows)
            };
        }
        
        // Check if it's ReplyKeyboardRemove
        if (json.TryGetProperty("remove_keyboard", out var removeKeyboard) && removeKeyboard.GetBoolean())
        {
            var selective = json.TryGetProperty("selective", out var sel) && sel.GetBoolean();
            return new TReplyKeyboardHide { Selective = selective };
        }
        
        // Check if it's ForceReply
        if (json.TryGetProperty("force_reply", out var forceReply) && forceReply.GetBoolean())
        {
            var selective = json.TryGetProperty("selective", out var sel) && sel.GetBoolean();
            return new TReplyKeyboardForceReply { Selective = selective };
        }
        
        return null;
    }

    /// <summary>
    /// Get optional int parameter from JSON
    /// </summary>
    public static int? GetOptionalInt(JsonElement body, string propertyName)
    {
        if (body.TryGetProperty(propertyName, out var prop) && prop.ValueKind != JsonValueKind.Null)
        {
            return prop.GetInt32();
        }
        return null;
    }

    /// <summary>
    /// Get optional string parameter from JSON
    /// </summary>
    public static string? GetOptionalString(JsonElement body, string propertyName)
    {
        if (body.TryGetProperty(propertyName, out var prop) && prop.ValueKind != JsonValueKind.Null)
        {
            return prop.GetString();
        }
        return null;
    }

    /// <summary>
    /// Get optional bool parameter from JSON
    /// </summary>
    public static bool GetOptionalBool(JsonElement body, string propertyName, bool defaultValue = false)
    {
        if (body.TryGetProperty(propertyName, out var prop) && prop.ValueKind != JsonValueKind.Null)
        {
            return prop.GetBoolean();
        }
        return defaultValue;
    }
}
