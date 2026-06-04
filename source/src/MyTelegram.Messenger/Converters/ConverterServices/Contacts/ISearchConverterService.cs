namespace MyTelegram.Messenger.Converters.ConverterServices.Contacts;

public interface ISearchConverterService
{
    IFound ToFound(IRequestWithAccessHashKeyId request, SearchContactOutput output, int layer);
}