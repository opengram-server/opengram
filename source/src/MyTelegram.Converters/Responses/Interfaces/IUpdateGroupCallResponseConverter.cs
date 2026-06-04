namespace MyTelegram.Converters.Responses.Interfaces;

public interface IUpdateGroupCallResponseConverter
    : IResponseConverter<
        TUpdateGroupCall,
        IUpdate
    >
{
}