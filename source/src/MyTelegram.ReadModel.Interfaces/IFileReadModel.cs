namespace MyTelegram.ReadModel.Interfaces;

public interface IFileReadModel : IReadModel
{
    //long UserId { get; }
    string Id { get; }
    long FileId { get; }
    long AccessHash { get; }
    byte[] Key { get; }
    byte[] Iv { get; }
    int Date { get; }
    //bool CdnSupported { get; }
}