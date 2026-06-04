namespace MyTelegram.ReadModel.Interfaces;

public interface IUserThemeReadModel : IReadModel
{
    long UserId { get; }
    long ThemeId { get; }
    string? Format { get; }
}