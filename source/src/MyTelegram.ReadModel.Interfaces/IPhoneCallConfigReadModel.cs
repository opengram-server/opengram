namespace MyTelegram.ReadModel.Interfaces;

public interface IPhoneCallConfigReadModel : IReadModel
{
    int ConfigVersion { get; }
    string Id { get; }
    byte[] RandomBytes { get; }
}