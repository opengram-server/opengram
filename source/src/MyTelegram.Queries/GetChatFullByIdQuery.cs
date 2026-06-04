using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.Queries;

public class GetChatFullByIdQuery(long chatId) : IQuery<IChatFullReadModel?>
{
    public long ChatId { get; } = chatId;
}
