namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Search for messages and peers globally
/// <para>Possible errors</para>
/// Code Type Description
/// 400 FOLDER_ID_INVALID Invalid folder ID.
/// 400 INPUT_FILTER_INVALID The specified filter is invalid.
/// 400 SEARCH_QUERY_EMPTY The search query is empty.
/// See <a href="https://corefork.telegram.org/method/messages.searchGlobal" />
///</summary>
internal sealed class SearchGlobalHandler(
    IMessageAppService messageAppService,
    IQueryProcessor queryProcessor,
    IGetHistoryConverterService getHistoryConverterService,
    ILogger<SearchGlobalHandler> logger)
    :
        RpcResultObjectHandler<RequestSearchGlobal, IMessages>,
        ISearchGlobalHandler
{
    protected override async Task<IMessages> HandleCoreAsync(IRequestInput input,
        RequestSearchGlobal obj)
    {
        var userId = input.UserId;

        // Search for users and channels by keyword
        var searchLimit = Math.Min(obj.Limit, 50);
        logger.LogInformation("SearchGlobal: Query='{Query}', Limit={Limit}", obj.Q, searchLimit);
        
        var users = await queryProcessor.ProcessAsync(new SearchUserByKeywordQuery(obj.Q, searchLimit));
        var channels = await queryProcessor.ProcessAsync(new SearchChannelByKeywordQuery(obj.Q, searchLimit));
        
        logger.LogInformation("SearchGlobal: Found {UserCount} users, {ChannelCount} channels", users.Count, channels.Count);

        var allJoinedChannelIdList =
            await queryProcessor.ProcessAsync(new GetAllJoinedChannelIdListQuery(input.UserId));

        var getMessageOutput = await messageAppService.SearchGlobalAsync(new SearchGlobalInput
        {
            OwnerPeerId = userId,
            SelfUserId = userId,
            Limit = obj.Limit,
            Q = obj.Q,
            FolderId = obj.FolderId,
            OffsetId = obj.OffsetId,
            JoinedChannelList = allJoinedChannelIdList.ToList(),
            BroadcastsOnly = obj.BroadcastsOnly,
            GroupsOnly = obj.GroupsOnly,
            UsersOnly = obj.UsersOnly
        });

        // Merge search results: add found users and channels to the output
        var mergedUserList = getMessageOutput.UserList.Concat(users).ToList();
        var mergedChannelList = getMessageOutput.ChannelList.Concat(channels).ToList();
        
        logger.LogInformation("SearchGlobal: Merging results - Original users: {OrigCount}, Found users: {FoundCount}, Total: {TotalCount}", 
            getMessageOutput.UserList.Count, users.Count, mergedUserList.Count);
        logger.LogInformation("SearchGlobal: AccessHashKeyId={AccessHashKeyId}, UserId={UserId}", 
            input.AccessHashKeyId, input.UserId);
        
        var mergedOutput = new GetMessageOutput(
            mergedChannelList,
            getMessageOutput.ChannelMemberList,
            getMessageOutput.ChatList,
            getMessageOutput.ContactList,
            getMessageOutput.JoinedChannelIdList,
            getMessageOutput.MessageList,
            getMessageOutput.PrivacyList,
            mergedUserList,
            getMessageOutput.PhotoList,
            getMessageOutput.PollList,
            getMessageOutput.ChosenPollOptions,
            getMessageOutput.UserReactionList,
            getMessageOutput.HasMoreData,
            getMessageOutput.IsSearchGlobal,
            getMessageOutput.Pts,
            getMessageOutput.SelfUserId,
            getMessageOutput.Limit,
            getMessageOutput.OffsetInfo
        );

        var result = getHistoryConverterService.ToMessages(input, mergedOutput, input.Layer);
        logger.LogInformation("SearchGlobal: Returning result type={ResultType}", result.GetType().Name);
        return result;
    }
}