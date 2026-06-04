namespace MyTelegram.Messenger.Converters.ConverterServices;

public interface IDialogConverterService
{
    IDialogs ToDialogs(IRequestWithAccessHashKeyId request, GetDialogOutput output, int layer = 0);
    IPeerDialogs ToPeerDialogs(IRequestWithAccessHashKeyId request, GetDialogOutput output, int layer = 0);
}