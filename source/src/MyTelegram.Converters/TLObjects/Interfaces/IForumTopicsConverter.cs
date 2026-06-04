namespace MyTelegram.Converters.TLObjects.Interfaces;

public interface IForumTopicsConverter : ILayeredConverter
{
    IForumTopic ToForumTopic(IForumTopicReadModel readModel);
}