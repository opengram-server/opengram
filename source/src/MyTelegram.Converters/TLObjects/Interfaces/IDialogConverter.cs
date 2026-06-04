namespace MyTelegram.Converters.TLObjects.Interfaces;

public interface IDialogConverter : ILayeredConverter
{
    IDialog ToDialog(IDialogReadModel readModel, IChannelReadModel? channelReadModel,
        IPeerNotifySettings peerNotifySettings);
}