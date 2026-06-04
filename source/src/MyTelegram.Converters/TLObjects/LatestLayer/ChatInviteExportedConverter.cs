//

//using MyTelegram.Schema.Messages;
//using IExportedChatInvite = MyTelegram.Schema.IExportedChatInvite;

//namespace MyTelegram.Converters.TLObjects;

//public partial interface IChatInviteExportedConverter
//{
//    IExportedChatInvite ToExportedChatInvite(IChatInviteReadModel readModel);
//}

//internal sealed class ChatInviteExportedConverter(IObjectMapper objectMapper) : IChatInviteExportedConverter, ITransientDependency
//{
//    public int Layer => Layers.LayerLatest;
//    public IExportedChatInvite ToExportedChatInvite(IChatInviteReadModel readModel)
//    {
//        return objectMapper.Map<IChatInviteReadModel, TChatInviteExported>(readModel);
//    }

//    

//}

