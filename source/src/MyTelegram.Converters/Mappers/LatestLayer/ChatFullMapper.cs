using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.Converters.Mappers.LatestLayer;

internal sealed class ChatFullMapper
    : IObjectMapper<IChatFullReadModel, TChatFull>,
        ILayeredMapper,
        ITransientDependency
{
    public int Layer => Layers.LayerLatest;

    public TChatFull Map(IChatFullReadModel source)
    {
        return Map(source, new TChatFull());
    }

    public TChatFull Map(
        IChatFullReadModel source,
        TChatFull destination
    )
    {
        destination.Id = source.ChatId;
        destination.About = source.About ?? string.Empty;
        destination.PinnedMsgId = source.PinnedMsgId;
        destination.FolderId = source.FolderId;
        destination.TtlPeriod = source.TtlPeriod;
        destination.RequestsPending = source.RequestsPending;
        destination.RecentRequesters = source.RecentRequesters != null 
            ? new TVector<long>(source.RecentRequesters) 
            : null;

        // Set available reactions based on ReactionType
        switch (source.ReactionType)
        {
            case ReactionType.ReactionNone:
                // Enable all reactions by default even for ReactionNone
                destination.AvailableReactions = new TChatReactionsAll
                {
                    AllowCustom = true
                };
                break;
            case ReactionType.ReactionAll:
                destination.AvailableReactions = new TChatReactionsAll
                {
                    AllowCustom = source.AllowCustomReaction
                };
                break;
            case ReactionType.ReactionSome:
                if (source.AvailableReactions?.Count > 0)
                {
                    destination.AvailableReactions = new TChatReactionsSome
                    {
                        Reactions = new TVector<IReaction>(source.AvailableReactions.Select(p => new TReactionEmoji
                        {
                            Emoticon = p
                        }))
                    };
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return destination;
    }
}
