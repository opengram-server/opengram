namespace MyTelegram.BotApi.Services;

/// <summary>
/// Проверка и очистка входных данных от пользователя
/// перед дальнейшей обработкой.
/// </summary>
public static class InputValidationService
{
    // Ограничения длины
    public const int MaxMessageTextLength = 4096; // лимит Telegram
    public const int MaxUrlLength = 2048;
    public const int MaxSearchKeywordLength = 256;
    public const int MaxUsernameLength = 32;
    public const int MaxTitleLength = 255;
    
    /// <summary>
    /// Проверяет текст сообщения и убирает из него опасные символы
    /// </summary>
    public static (bool IsValid, string? ErrorMessage, string SanitizedValue) ValidateMessageText(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return (false, "Text cannot be empty", string.Empty);
        
        if (text.Length > MaxMessageTextLength)
            return (false, $"Text too long (max {MaxMessageTextLength} characters)", string.Empty);
        
        // \u0423\u0434\u0430\u043B\u044F\u0435\u043C \u043D\u0443\u043B\u0435\u0432\u044B\u0435 \u0431\u0430\u0439\u0442\u044B \u0438 \u0441\u0438\u043C\u0432\u043E\u043B \u0437\u0430\u043C\u0435\u043D\u044B
        var sanitized = text.Replace("\0", "").Replace("\uFFFD", "");
        
        return (true, null, sanitized);
    }
    
    /// <summary>
    /// Проверяет корректность URL
    /// </summary>
    public static (bool IsValid, string? ErrorMessage) ValidateUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return (true, null); // URL необязателен
        
        if (url.Length > MaxUrlLength)
            return (false, $"URL too long (max {MaxUrlLength} characters)");
        
        // Допускаем только абсолютные ссылки по схеме HTTP(S)
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return (false, "Invalid URL format");

        if (uri.Scheme != "http" && uri.Scheme != "https")
            return (false, "Only HTTP(S) URLs allowed");

        // Запрещаем localhost и внутренние адреса, чтобы исключить SSRF
        if (uri.Host == "localhost" || 
            uri.Host == "127.0.0.1" || 
            uri.Host.StartsWith("192.168.") ||
            uri.Host.StartsWith("10.") ||
            uri.Host.StartsWith("172."))
        {
            return (false, "Internal URLs not allowed");
        }
        
        return (true, null);
    }
    
    /// <summary>
    /// Очищает поисковый запрос, убирая символы, способные вызвать ReDoS
    /// </summary>
    public static (bool IsValid, string? ErrorMessage, string SanitizedValue) SanitizeSearchKeyword(string? keyword)
    {
        if (string.IsNullOrEmpty(keyword))
            return (false, "Search keyword cannot be empty", string.Empty);
        
        if (keyword.Length > MaxSearchKeywordLength)
            return (false, $"Search keyword too long (max {MaxSearchKeywordLength} characters)", string.Empty);
        
        // Убираем спецсимволы регулярных выражений, способные вызвать ReDoS
        var dangerous = new[] { '*', '.', '+', '?', '^', '$', '(', ')', '[', ']', '{', '}', '|', '\\' };
        var sanitized = keyword;
        foreach (var ch in dangerous)
        {
            sanitized = sanitized.Replace(ch.ToString(), "");
        }
        
        // Убираем пробелы по краям
        sanitized = sanitized.Trim();
        
        if (string.IsNullOrEmpty(sanitized))
            return (false, "Search keyword contains only special characters", string.Empty);
        
        return (true, null, sanitized);
    }
    
    /// <summary>
    /// Проверяет идентификатор чата
    /// </summary>
    public static (bool IsValid, string? ErrorMessage) ValidateChatId(long chatId)
    {
        // Нулевой идентификатор недопустим
        if (chatId == 0)
            return (false, "Invalid chat_id");
        
        return (true, null);
    }
    
    /// <summary>
    /// Проверяет имя пользователя (username)
    /// </summary>
    public static (bool IsValid, string? ErrorMessage, string SanitizedValue) ValidateUsername(string? username)
    {
        if (string.IsNullOrEmpty(username))
            return (true, null, string.Empty);
        
        if (username.Length > MaxUsernameLength)
            return (false, $"Username too long (max {MaxUsernameLength} characters)", string.Empty);
        
        // Допустимы только латинские буквы, цифры и знак подчёркивания
        if (!System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"))
            return (false, "Username can only contain letters, numbers and underscore", string.Empty);
        
        return (true, null, username);
    }
}
