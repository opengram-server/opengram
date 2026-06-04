namespace MyTelegram.Converters.Responses;

public interface IDialogFilterResponseService
{
    IDialogFilter? ToLayeredData(IDialogFilter? latestLayerData, int layer);
}