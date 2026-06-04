namespace MyTelegram;

public record WallPaper(long Id, long AccessHash, bool Default, bool Pattern, bool Dark, string Slug, long? DocumentId, WallPaperSettings? Settings);