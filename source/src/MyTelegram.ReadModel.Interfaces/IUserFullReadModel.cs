namespace MyTelegram.ReadModel.Interfaces;

public interface IUserFullReadModel : IReadModel
{
    string Id { get; }
    long UserId { get; }
    IBusinessWorkHours? BusinessWorkHours { get; set; }
    IBusinessLocation? BusinessLocation { get; }
    IBusinessGreetingMessage? BusinessGreetingMessage { get; set; }
    IBusinessAwayMessage? BusinessAwayMessage { get; set; }
    IBusinessIntro? BusinessIntro { get; }
    bool IsBusinessAccount { get; set; }
    long? PersonalChannelId { get; }
    int? PersonalChannelMessage { get; }
}