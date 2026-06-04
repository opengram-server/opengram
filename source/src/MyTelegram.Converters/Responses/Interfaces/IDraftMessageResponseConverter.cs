// ReSharper disable All

namespace MyTelegram.Converters.Responses;

public partial interface IDraftMessageResponseConverter
    : IResponseConverter<
        TDraftMessage,
        IDraftMessage
    >
{
}