namespace MyTelegram;

public record PhotoSize(int W, int H, long Size, string Type, byte[]? StrippedThumb = null, byte[]? Bytes = null);