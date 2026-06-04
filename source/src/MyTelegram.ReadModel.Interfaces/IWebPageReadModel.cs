namespace MyTelegram.ReadModel.Interfaces;

public interface IWebPageReadModel : IReadModel
{
    long WebPageId { get; }
    string Url { get; }
    string DisplayUrl { get; }
    int Hash { get; }
    string Type { get; }
    string SiteName { get; }
    string Title { get; }
    string Description { get; }
    IPhoto? Photo { get; }
    int Date { get; }
}