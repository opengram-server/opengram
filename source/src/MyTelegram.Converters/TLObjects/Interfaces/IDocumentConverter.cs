namespace MyTelegram.Converters.TLObjects.Interfaces;

public interface IDocumentConverter : ILayeredConverter
{
    ILayeredDocument ToDocument(IDocumentReadModel documentReadModel);
}