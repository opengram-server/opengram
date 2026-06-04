namespace MyTelegram.ReadModel.Interfaces;

public interface IDialogFilterReadModel : IReadModel
{
    long OwnerUserId { get; }
    bool IsShareableFolder { get; }
    DialogFilter Filter { get; }
    string? ImportedFromSlug { get; }
}
