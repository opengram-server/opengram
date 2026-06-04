namespace MyTelegram.ReadModel.Interfaces;

public interface IRpcResultReadModel : IReadModel
{
    string Id { get; }
    long UserId { get; }
    long ReqMsgId { get; }
    byte[] RpcData { get; }
    //string SourceId { get; }
    int Date { get; }
}