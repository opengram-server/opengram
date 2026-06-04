//using EventFlow.Extensions;
//using Microsoft.Extensions.DependencyInjection;
//using MyTelegram.Domain.Aggregates.Channel;
//using MyTelegram.Domain.Aggregates.Messaging;
//using MyTelegram.Domain.Aggregates.User;
//using MyTelegram.Domain.Commands.Channel;
//using MyTelegram.Domain.Commands.Messaging;
//using MyTelegram.Domain.Commands.User;
//using MyTelegram.Queries;
//using MyTelegram.QueryHandlers.InMemory;
//using MyTelegram.ReadModel.InMemory;
//using MyTelegram.TestBase;
//using Shouldly;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using MyTelegram.Domain.Aggregates.Temp;
//using Xunit;

//namespace MyTelegram.Domain.IntegrationTests.Aggregates.Sagas;

//public class MessageSagaIntegrationTests : IntegrationTest
//{
//    [Fact]
//    public async Task SendMessage_To_UserPeer_Test()
//    {
//        var senderPeerId = 1;
//        var recipientPeerId = 2;
//        var message = A<string>();
//        await CreateUserAsync(senderPeerId);
//        await CreateUserAsync(recipientPeerId);

//        await SendMessageToPeerAsync(senderPeerId.ToUserPeer(), recipientPeerId.ToUserPeer(), message);

//        var outboxMessageReadModel = await GetLatestMessageAsync(senderPeerId);
//        var inboxMessageReadModel = await GetLatestMessageAsync(recipientPeerId);
//        outboxMessageReadModel.Message.ShouldBe(message);
//        outboxMessageReadModel.Out.ShouldBeTrue();
//        inboxMessageReadModel.Message.ShouldBe(message);
//        inboxMessageReadModel.Out.ShouldBeFalse();
//    }

//    [Fact]
//    public async Task SendMessage_To_ChatPeer_Test()
//    {
//        var senderPeerId = 1;
//        var recipientPeerId = 2L;
//        var message = A<string>();
//        var chatId = MyTelegramConsts.ChatIdInitId + 1;
//        await CreateUserAsync(senderPeerId);
//        await CreateUserAsync(recipientPeerId);
//        await CreateChatAsync(chatId, senderPeerId, new[] { recipientPeerId });

//        await SendMessageToPeerAsync(senderPeerId.ToUserPeer(), chatId.ToChatPeer(), message);

//        var member1MessageReadModel = await GetLatestMessageAsync(senderPeerId);
//        var member2MessageReadModel = await GetLatestMessageAsync(recipientPeerId);
//        member1MessageReadModel.Out.ShouldBeTrue();
//        member1MessageReadModel.SenderPeerId.ShouldBe(senderPeerId);
//        member1MessageReadModel.ToPeerId.ShouldBe(chatId);
//        member1MessageReadModel.Message.ShouldBe(message);
//        member2MessageReadModel.Out.ShouldBeFalse();
//        member2MessageReadModel.Message.ShouldBe(message);
//        member2MessageReadModel.SenderPeerId.ShouldBe(senderPeerId);
//        member2MessageReadModel.ToPeerId.ShouldBe(chatId);
//    }

//    [Fact]
//    public async Task SendMessage_To_ChannelPeer_Test()
//    {
//        // Arrange
//        var senderPeerId = 1;
//        var recipientPeerId = 2;
//        var message = A<string>();
//        var channelId = MyTelegramConsts.ChannelInitId + 1;
//        await CreateUserAsync(senderPeerId);
//        await CreateUserAsync(recipientPeerId);
//        await CreateChannelAsync(channelId, senderPeerId);
//        await AddChannelMemberAsync(channelId, recipientPeerId, senderPeerId);

//        // Act
//        await SendMessageToPeerAsync(senderPeerId.ToUserPeer(), channelId.ToChannelPeer(), message);

//        // Assert
//        var readModel = await GetLatestMessageAsync(channelId);
//        readModel.Message.ShouldBe(message);
//        readModel.Out.ShouldBeTrue();
//        readModel.OwnerPeerId.ShouldBe(channelId);
//        readModel.SenderPeerId.ShouldBe(senderPeerId);
//        var channelReadModel = await QueryProcessor.ProcessAsync(new GetChannelByIdQuery(channelId), default)
//            ;
//        channelReadModel.TopMessageId.ShouldBe(readModel.MessageId);
//    }

//    private async Task<IMessageReadModel> GetLatestMessageAsync(long ownerPeerId)
//    {
//        var readModels = await QueryProcessor.ProcessAsync(new GetMessagesQuery(ownerPeerId,
//            MessageType.Text,
//            null,
//            null,
//            0,
//            10,
//            null,
//            null,
//            0,
//            0), default);

//        var maxMessageId = readModels.Max(p => p.MessageId);
//        return readModels.Single(p => p.MessageId == maxMessageId);
//    }

//    private Task AddChannelMemberAsync(long channelId,
//        long memberPeerId,
//        long inviterId)
//    {
//        var command = new StartInviteToChannelCommand(TempId.New,
//            A<RequestInfo>() with { UserId = inviterId },
//            channelId,
//            true,
//            true,
//            inviterId,
//            new[] { memberPeerId },
//            Array.Empty<long>(),
//            //A<int>(),
//            //A<long>(),
//            Array.Empty<long>(),
//            A<int>(),
//            A<long>(),
//             ChatJoinType.InvitedByAdmin);
//        return CommandBus.PublishAsync(command, default);
//    }

//    private Task SendMessageToPeerAsync(Peer senderPeer, Peer toPeer, string message)
//    {
//        var randomId = A<long>();
//        var messageId = A<int>();
//        var aggregateId = MessageId.Create(senderPeer.PeerId, messageId);
//        var item = new MessageItem(
//            toPeer.PeerType == PeerType.Channel ? toPeer : senderPeer,
//            toPeer,
//            senderPeer,
//            senderPeer.PeerId,
//            messageId,
//            message,
//            A<int>(),
//            randomId,
//            true);
//        var command = new CreateOutboxMessageCommand(aggregateId, A<RequestInfo>() with { UserId = senderPeer.PeerId }, item);
//        return CommandBus.PublishAsync(command, default);
//    }

//    private Task CreateChannelAsync(long channelId, long creatorId)
//    {
//        var title = A<string>();
//        var command = new CreateChannelCommand(ChannelId.Create(channelId),
//            A<RequestInfo>() with { UserId = creatorId },
//            channelId,
//            creatorId,
//            title,
//            true,
//            false,
//            null,
//            null,
//            0,
//            A<int>(),
//            A<long>(),
//            A<string>(),
//            0,
//            false,
//            null, null, null
//            );
//        return CommandBus.PublishAsync(command, default);
//    }

//    private Task CreateChatAsync(long chatId, long creatorId, IReadOnlyList<long> memberUidList)
//    {
//        var command = new CreateChatCommand(ChatId.Create(chatId),
//            A<RequestInfo>() with { UserId = creatorId },
//            chatId,
//            creatorId,
//            A<string>(),
//            memberUidList,
//            A<int>(),
//            A<long>(),
//            A<string>());
//        return CommandBus.PublishAsync(command, default);
//    }

//    private Task CreateUserAsync(long userId)
//    {
//        var command = new CreateUserCommand(UserId.Create(userId),
//            A<RequestInfo>(),
//            userId,
//            1,
//            userId.ToString(),
//            userId.ToString(),
//            null);
//        return CommandBus.PublishAsync(command, default);
//    }

//    protected override IServiceProvider Configure(IEventFlowOptions options)
//    {
//        options.AddDefaults(typeof(MyTelegram.Domain.IChatInviteLinkHelper).Assembly);
//        options.ServiceCollection.AddMyEventFlow();
//        options.ServiceCollection.AddSingleton<IIdGenerator, SimpleInMemoryIdGenerator>();
//        options.AddInMemoryReadModel();
//        options.AddInMemoryQueryHandlers();

//        var serviceProvider = options.ServiceCollection.BuildServiceProvider();

//        return serviceProvider;
//    }
//}