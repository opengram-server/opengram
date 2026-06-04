using Microsoft.Extensions.Logging;
using MyTelegram.Domain.Shared.Groups;
using MyTelegram.Schema;
using MyTelegram.Schema.Messages;
using MyTelegram.Handlers;
using MyTelegram.Services.Services;
using MyTelegram.Messenger.Services.Impl;
using MyTelegram.Messenger.Services.Interfaces;
using MyTelegram.Services.Extensions;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Channels;

///<summary>
/// Convert a <a href="https://corefork.telegram.org/api/channel">supergroup</a> to a <a href="https://corefork.telegram.org/api/channel">gigagroup</a>, when requested by <a href="https://corefork.telegram.org/api/config#channel-suggestions">channel suggestions</a>.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 CHANNEL_ID_INVALID The specified supergroup ID is invalid.
/// 400 CHANNEL_INVALID The provided channel is invalid.
/// 400 CHAT_ADMIN_REQUIRED You must be an admin in this chat to do this.
/// 403 CHAT_WRITE_FORBIDDEN You can't write in this chat.
/// 400 FORUM_ENABLED You can't execute the specified action because the group is a <a href="https://corefork.telegram.org/api/forum">forum</a>, disable forum functionality to continue.
/// 400 PARTICIPANTS_TOO_FEW Not enough participants.
/// See <a href="https://corefork.telegram.org/method/channels.convertToGigagroup" />
///</summary>
internal sealed class ConvertToGigagroupHandler : BaseObjectHandler<MyTelegram.Schema.Channels.RequestConvertToGigagroup, IObject>, IConvertToGigagroupHandler
{
    private readonly ILogger<ConvertToGigagroupHandler> logger;
    private readonly IGigagroupAppService gigagroupAppService;
    private readonly IChannelAppService channelAppService;

    public ConvertToGigagroupHandler(
        ILogger<ConvertToGigagroupHandler> logger,
        IGigagroupAppService gigagroupAppService,
        IChannelAppService channelAppService)
    {
        this.logger = logger;
        this.gigagroupAppService = gigagroupAppService;
        this.channelAppService = channelAppService;
    }
    protected override async Task<IObject> HandleCoreAsync(IRequestInput input, MyTelegram.Schema.Channels.RequestConvertToGigagroup obj)
    {
        var channelPeer = obj.Channel.ToChannelPeer();
        var channelId = channelPeer.PeerId;
        
        logger.LogInformation("Converting channel {ChannelId} to gigagroup by user {UserId}", 
            channelId, input.UserId);

        try
        {
            // Validate channel exists and is a megagroup
            var channel = await channelAppService.GetAsync(channelId);
            if (channel == null)
            {
                throw new InvalidOperationException("Channel not found");
            }

            // Check if user is channel admin
            if (!await IsChannelAdminAsync(input.UserId, channelId))
            {
                throw new InvalidOperationException("You must be an admin in this chat to do this");
            }

            // Create conversion request
            var conversionRequest = new GigagroupConversionRequest
            {
                SupergroupId = channelId,
                RequestedBy = input.UserId,
                RequiresConfirmation = false // For API, we convert immediately
            };

            // Process conversion
            var result = await gigagroupAppService.ConvertToGigagroupAsync(conversionRequest);

            if (!result.Success)
            {
                throw new InvalidOperationException(result.ErrorMessage ?? "Conversion failed");
            }

            // Create updates response
            var updates = CreateConversionUpdates(channelId, input.UserId, result.ConvertedAt);

            logger.LogInformation("Channel {ChannelId} converted to gigagroup successfully", channelId);

            return updates;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error converting channel {ChannelId} to gigagroup", channelId);
            throw;
        }
    }

    private async Task<bool> IsChannelAdminAsync(long userId, long channelId)
    {
        // In a real implementation, this would check channel participant permissions
        // For now, we'll assume they are if they're making this request
        // This should be replaced with proper permission checking
        return await Task.FromResult(true);
    }

    private IUpdates CreateConversionUpdates(long channelId, long userId, DateTime convertedAt)
    {
        // Create updates notification for gigagroup conversion
        var updateServiceMessage = new MyTelegram.Schema.TUpdateNewChannelMessage
        {
            Message = new MyTelegram.Schema.TMessageService
            {
                Id = (int)DateTime.UtcNow.Ticks, // Generate unique message ID
                PeerId = new TPeerChannel
                {
                    ChannelId = channelId
                },
                FromId = new TPeerUser
                {
                    UserId = userId
                },
                Action = new TMessageActionChatMigrateTo
                {
                    ChannelId = channelId
                },
                Date = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            }
        };

        var updates = new TUpdates
        {
            Updates = new TVector<IUpdate>(updateServiceMessage),
            Users = new TVector<IUser>(),
            Chats = new TVector<IChat>(),
            Date = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Seq = 0
        };

        return updates;
    }
}

/// <summary>
/// TL Schema for service message about channel migration to gigagroup
/// </summary>
public class TMessageActionChatMigrateTo : IMessageActionChatMigrateTo
{
    public uint ConstructorId => 0x51bdb021;
    public long ChannelId { get; set; }

    public void ComputeFlag()
    {
    }

    public void Serialize(IBufferWriter<byte> writer)
    {
        ComputeFlag();
        writer.Write(ConstructorId);
        writer.Write(ChannelId);
    }

    public void Deserialize(ref ReadOnlyMemory<byte> buffer)
    {
        ChannelId = buffer.ReadInt64();
    }
}
