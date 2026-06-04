// ReSharper disable All

namespace MyTelegram.Converters.Responses;

public partial interface IMessageEntityBlockquoteResponseConverter
    : IResponseConverter<
        TMessageEntityBlockquote,
        IMessageEntity
    >
{
}