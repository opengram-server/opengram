namespace MyTelegram.Converters.TLObjects.Interfaces;

public interface IDialogFilterConverter : ILayeredConverter
{
    IDialogFilter ToDialogFilter(DialogFilter dialogFilter);
}