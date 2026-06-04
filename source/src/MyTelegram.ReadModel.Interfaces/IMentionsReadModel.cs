namespace MyTelegram.ReadModel.Interfaces;

public interface IMentionsReadModel : IReadModel
{
    long OwnerUserId { get; }
    public PeerType ToPeerType { get; set; }
    public long ToPeerId { get; set; }
    //Peer ToPeer { get; }
    int MessageId { get; }
}