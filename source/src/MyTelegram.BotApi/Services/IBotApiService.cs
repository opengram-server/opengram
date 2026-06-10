using MyTelegram.Domain.Shared.BotApi;
using System.Text.Json;

namespace MyTelegram.BotApi.Services;

public interface IBotApiService
{
    // Проверка токена бота
    Task<bool> ValidateBotTokenAsync(string token);
    
    Task<BotApiUser> GetMeAsync(string token);
    Task<List<BotApiUpdate>> GetUpdatesAsync(string token, int offset, int limit, int timeout);
    Task<BotApiMessage> SendMessageAsync(string token, JsonElement body);
    Task<BotApiMessage> ForwardMessageAsync(string token, JsonElement body);
    Task<int> CopyMessageAsync(string token, JsonElement body);
    Task<bool> SetWebhookAsync(string token, JsonElement body);
    Task<bool> DeleteWebhookAsync(string token);
    Task<object> GetWebhookInfoAsync(string token);
    Task<object> GetAvailableGiftsAsync(string token);
    Task<bool> SendGiftAsync(string token, JsonElement body);
    Task<BotApiMessage> SendPhotoAsync(string token, long chatId, string? photo, IFormFile? photoFile);
    Task<BotApiMessage> SendAudioAsync(string token, long chatId, string? audio, IFormFile? audioFile);
    Task<BotApiMessage> SendDocumentAsync(string token, long chatId, string? document, IFormFile? documentFile);
    Task<BotApiMessage> SendVideoAsync(string token, long chatId, string? video, IFormFile? videoFile);
    Task<BotApiMessage> SendAnimationAsync(string token, long chatId, string? animation, IFormFile? animationFile);
    Task<BotApiMessage> SendVoiceAsync(string token, long chatId, string? voice, IFormFile? voiceFile);
    Task<BotApiMessage> SendVideoNoteAsync(string token, long chatId, string? videoNote, IFormFile? videoNoteFile);
    Task<List<BotApiMessage>> SendMediaGroupAsync(string token, JsonElement body);
    Task<BotApiMessage> SendLocationAsync(string token, JsonElement body);
    Task<BotApiMessage> SendVenueAsync(string token, JsonElement body);
    Task<BotApiMessage> SendContactAsync(string token, JsonElement body);
    Task<BotApiMessage> SendPollAsync(string token, JsonElement body);
    Task<BotApiMessage> SendDiceAsync(string token, JsonElement body);
    Task SendChatActionAsync(string token, JsonElement body);
    Task<BotApiMessage> EditMessageTextAsync(string token, JsonElement body);
    Task<BotApiMessage> EditMessageReplyMarkupAsync(string token, JsonElement body);
    Task<bool> DeleteMessageAsync(string token, JsonElement body);
    Task<bool> AnswerCallbackQueryAsync(string token, JsonElement body);
    Task<BotApiMessage> SendStickerAsync(string token, long chatId, string? sticker, IFormFile? stickerFile);
    Task<object> GetUserProfilePhotosAsync(string token, long userId, int? offset, int? limit);
    Task<object> GetFileAsync(string token, string fileId);
    Task BanChatMemberAsync(string token, JsonElement body);
    Task UnbanChatMemberAsync(string token, JsonElement body);
    Task RestrictChatMemberAsync(string token, JsonElement body);
    Task PromoteChatMemberAsync(string token, JsonElement body);
    Task SetChatAdministratorCustomTitleAsync(string token, JsonElement body);
    Task BanChatSenderChatAsync(string token, JsonElement body);
    Task UnbanChatSenderChatAsync(string token, JsonElement body);
    Task SetChatPermissionsAsync(string token, JsonElement body);
    Task<string> ExportChatInviteLinkAsync(string token, JsonElement body);
    Task<object> CreateChatInviteLinkAsync(string token, JsonElement body);
    Task<object> EditChatInviteLinkAsync(string token, JsonElement body);
    Task<object> RevokeChatInviteLinkAsync(string token, JsonElement body);
    Task ApproveChatJoinRequestAsync(string token, JsonElement body);
    Task DeclineChatJoinRequestAsync(string token, JsonElement body);
    Task SetChatPhotoAsync(string token, long chatId, IFormFile photo);
    Task DeleteChatPhotoAsync(string token, JsonElement body);
    Task SetChatTitleAsync(string token, JsonElement body);
    Task SetChatDescriptionAsync(string token, JsonElement body);
    Task<BotApiChat> GetChatAsync(string token, JsonElement body);
    
    // Оплата звёздами (Stars)
    Task<BotApiMessage> SendInvoiceAsync(string token, JsonElement body);
    Task<bool> AnswerPreCheckoutQueryAsync(string token, JsonElement body);
}
