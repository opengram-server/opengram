using EventFlow.Queries;
using MyTelegram.EventFlow.ReadStores;
using MyTelegram.Messenger.Services.Impl;
using MyTelegram.ReadModel;

namespace MyTelegram.Messenger.Services.Impl;

public class GetMonoforumQueryHandler(IQueryOnlyReadModelStore<ChannelReadModel> store) : IQueryHandler<GetMonoforumQuery, Monoforum>
{
    public async Task<Monoforum> ExecuteQueryAsync(GetMonoforumQuery query, CancellationToken cancellationToken)
    {
        var channel = await store.FirstOrDefaultAsync(p => p.ChannelId == query.ChannelId, cancellationToken);
        if (channel == null)
        {
            return null;
        }

        // If this channel is a monoforum, return it directly
        if (channel.Monoforum)
        {
            return MapToMonoforum(channel);
        }

        // If this channel is linked to a monoforum, return the linked monoforum
        if (channel.LinkedMonoforumId.HasValue)
        {
            var monoforumChannel = await store.FirstOrDefaultAsync(p => p.ChannelId == channel.LinkedMonoforumId.Value, cancellationToken);
            if (monoforumChannel != null)
            {
                return MapToMonoforum(monoforumChannel);
            }
        }

        return null;
    }

    private Monoforum MapToMonoforum(IChannelReadModel channel)
    {
        return new Monoforum
        {
            Id = channel.ChannelId.ToString(),
            ChannelId = channel.ChannelId,
            CreatorId = channel.CreatorId,
            IsMonoforum = channel.Monoforum,
            CreatedAt = DateTime.UtcNow, // Not stored in read model
            UpdatedAt = DateTime.UtcNow,
            Settings = new MonoforumSettings
            {
                AllowPublicTopics = false, // Default
                AutoModerateMessages = true // Default
            }
        };
    }
}
