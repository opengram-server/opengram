namespace MyTelegram.ReadModel.Interfaces;

public interface IThemeReadModel : IReadModel
{
    long CreatorUserId { get; }
    long ThemeId { get; }
    string? Emoticon { get; }
    Theme Theme { get; }
}