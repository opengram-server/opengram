namespace MyTelegram.ReadModel.Interfaces;

public interface IReactionReadModel : IReadModel
{
    string Emoji { get; }
    string Title { get; }
    bool Premium { get; }
    bool Inactive { get; }
    long? StaticIconDocumentId { get; }
    long? AppearAnimationDocumentId { get; }
    long? SelectAnimationDocumentId { get; }
    long? ActivateAnimationDocumentId { get; }
    long? EffectAnimationDocumentId { get; }
    long? AroundAnimationDocumentId { get; }
    long? CenterIconDocumentId { get; }
}