namespace MyTelegram.Messenger.Converters.ConverterServices.Messages;

internal sealed class GetHistoryConverterService(
    IUserConverterService userConverterService,
    IChatConverterService chatConverterService,
    IMessageConverterService messageConverterService,
    ILogger<GetHistoryConverterService> logger
    ) : IGetHistoryConverterService, ITransientDependency
{
    public IMessages ToMessages(IRequestWithAccessHashKeyId request, GetMessageOutput output, int layer)
    {
        var messages = messageConverterService.ToMessageList(output.SelfUserId,
            output.MessageList,
            output.PollList,
            output.ChosenPollOptions,
            output.UserReactionList,
            layer);

        var users = userConverterService.ToUserList(request,
            output.UserList,
            output.PhotoList,
            output.ContactList,
            output.PrivacyList,
            layer);

        var channels = chatConverterService.ToChannelList(request,
            output.ChannelList,
            output.PhotoList,
            output.ChannelMemberList,
            output.JoinedChannelIdList,
            layer);

        var hasMessages = messages.Count > 0;

        var offsetId = hasMessages ? output.MessageList.Max(p => p.MessageId) : 0;
        if (output.OffsetInfo?.LoadType == LoadType.Backward)
        {
            offsetId = hasMessages ? output.MessageList.Min(p => p.MessageId) : 0;
        }

        // For global search, return results even if no messages found (users/channels only)
        if (output.IsSearchGlobal && !hasMessages && (users.Count > 0 || channels.Count > 0))
        {
            logger.LogInformation("ToMessages: Global search with no messages - returning {UserCount} users, {ChannelCount} channels", 
                users.Count, channels.Count);
            
            // Log user details for debugging
            foreach (var user in users)
            {
                logger.LogInformation("User: Id={UserId}, AccessHash={AccessHash}, Username={Username}, FirstName={FirstName}", 
                    user.Id, user.AccessHash, user is TUser u ? u.Username : null, user.FirstName);
            }
            
            // Return TMessages (not TMessagesSlice) for global search with no messages
            // This tells the client that search is complete
            return new TMessages
            {
                Chats = [.. channels],
                Messages = [],
                Users = [.. users]
            };
        }

        if (hasMessages && output.MessageList.All(p => p.ToPeerType == PeerType.Channel) && !output.IsSearchGlobal)
        {
            var channelPts = output.ChannelList.FirstOrDefault()?.Pts ?? output.Pts;

            return new TChannelMessages
            {
                Chats = [.. channels],
                Messages = [.. messages],
                Users = [.. users],
                Pts = channelPts,
                Count = messages.Count,
                OffsetIdOffset = offsetId,
                Topics = [],
                Inexact = false
            };
        }

        if (messages.Count == output.Limit)
        {
            return new TMessagesSlice
            {
                Chats = [.. channels],
                Count = messages.Count,
                Inexact = true,
                NextRate = DateTime.UtcNow.AddSeconds(3).ToTimestamp(),
                Messages = [.. messages],
                Users = [.. users],
                OffsetIdOffset = offsetId,
            };
        }

        return new TMessages
        {
            Chats = [.. channels],
            Messages = [.. messages],
            Users = [.. users]
        };
    }
}
