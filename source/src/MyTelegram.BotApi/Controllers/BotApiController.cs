using Microsoft.AspNetCore.Mvc;
using MyTelegram.BotApi.Services;
using System.Text.Json;

namespace MyTelegram.BotApi.Controllers;

[ApiController]
public class BotApiController(
    IBotApiService botApiService,
    ILogger<BotApiController> logger) : ControllerBase
{
    private async Task<IActionResult?> ValidateTokenAsync(string botId, string secretToken)
    {
        var token = $"{botId}:{secretToken}";

        if (!await botApiService.ValidateBotTokenAsync(token))
        {
            logger.LogWarning("SECURITY: Invalid bot token attempted - BotId: {BotId}", botId);
            return Unauthorized(new { ok = false, error_code = 401, description = "Unauthorized: Invalid bot token" });
        }

        return null;
    }

    private async Task<IActionResult> ExecuteAsync<T>(string botId, string secretToken, Func<string, Task<T>> action, string method)
    {
        var authError = await ValidateTokenAsync(botId, secretToken);
        if (authError != null) return authError;

        try
        {
            var token = $"{botId}:{secretToken}";
            var result = await action(token);
            return Ok(new { ok = true, result });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in {Method}", method);
            return Ok(new { ok = false, error_code = 400, description = ex.Message });
        }
    }

    private async Task<IActionResult> ExecuteVoidAsync(string botId, string secretToken, Func<string, Task> action, string method)
    {
        var authError = await ValidateTokenAsync(botId, secretToken);
        if (authError != null) return authError;

        try
        {
            var token = $"{botId}:{secretToken}";
            await action(token);
            return Ok(new { ok = true, result = true });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in {Method}", method);
            return Ok(new { ok = false, error_code = 400, description = ex.Message });
        }
    }

    #region Basic Methods

    [HttpGet("bot{botId}:{secretToken}/getMe")]
    public Task<IActionResult> GetMe(string botId, string secretToken)
        => ExecuteAsync(botId, secretToken, t => botApiService.GetMeAsync(t), "getMe");

    [HttpGet("bot{botId}:{secretToken}/getUpdates")]
    public Task<IActionResult> GetUpdates(
        string botId, string secretToken,
        [FromQuery] int offset = 0, [FromQuery] int limit = 100, [FromQuery] int timeout = 0)
        => ExecuteAsync(botId, secretToken, t => botApiService.GetUpdatesAsync(t, offset, limit, timeout), "getUpdates");

    [HttpPost("bot{botId}:{secretToken}/setWebhook")]
    public Task<IActionResult> SetWebhook(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteAsync(botId, secretToken, t => botApiService.SetWebhookAsync(t, body), "setWebhook");

    [HttpPost("bot{botId}:{secretToken}/deleteWebhook")]
    public Task<IActionResult> DeleteWebhook(string botId, string secretToken)
        => ExecuteAsync(botId, secretToken, t => botApiService.DeleteWebhookAsync(t), "deleteWebhook");

    [HttpGet("bot{botId}:{secretToken}/getWebhookInfo")]
    public Task<IActionResult> GetWebhookInfo(string botId, string secretToken)
        => ExecuteAsync(botId, secretToken, t => botApiService.GetWebhookInfoAsync(t), "getWebhookInfo");

    #endregion

    #region Send Message Methods

    [HttpPost("bot{botId}:{secretToken}/sendMessage")]
    public Task<IActionResult> SendMessage(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteAsync(botId, secretToken, t => botApiService.SendMessageAsync(t, body), "sendMessage");

    [HttpPost("bot{botId}:{secretToken}/forwardMessage")]
    public Task<IActionResult> ForwardMessage(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteAsync(botId, secretToken, t => botApiService.ForwardMessageAsync(t, body), "forwardMessage");

    [HttpPost("bot{botId}:{secretToken}/copyMessage")]
    public Task<IActionResult> CopyMessage(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteAsync(botId, secretToken, t => botApiService.CopyMessageAsync(t, body), "copyMessage");

    #endregion

    #region Media Methods

    [HttpPost("bot{botId}:{secretToken}/sendPhoto")]
    public async Task<IActionResult> SendPhoto(string botId, string secretToken,
        [FromForm] long chat_id, [FromForm] string? photo = null, IFormFile? photoFile = null)
    {
        var authError = await ValidateTokenAsync(botId, secretToken);
        if (authError != null) return authError;

        try
        {
            var token = $"{botId}:{secretToken}";
            var result = await botApiService.SendPhotoAsync(token, chat_id, photo, photoFile);
            return Ok(new { ok = true, result });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in sendPhoto");
            return Ok(new { ok = false, error_code = 400, description = ex.Message });
        }
    }

    [HttpPost("bot{botId}:{secretToken}/sendAudio")]
    public async Task<IActionResult> SendAudio(string botId, string secretToken,
        [FromForm] long chat_id, [FromForm] string? audio = null, IFormFile? audioFile = null)
    {
        var authError = await ValidateTokenAsync(botId, secretToken);
        if (authError != null) return authError;

        try
        {
            var token = $"{botId}:{secretToken}";
            var result = await botApiService.SendAudioAsync(token, chat_id, audio, audioFile);
            return Ok(new { ok = true, result });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in sendAudio");
            return Ok(new { ok = false, error_code = 400, description = ex.Message });
        }
    }

    [HttpPost("bot{botId}:{secretToken}/sendDocument")]
    public async Task<IActionResult> SendDocument(string botId, string secretToken,
        [FromForm] long chat_id, [FromForm] string? document = null, IFormFile? documentFile = null)
    {
        var authError = await ValidateTokenAsync(botId, secretToken);
        if (authError != null) return authError;

        try
        {
            var token = $"{botId}:{secretToken}";
            var result = await botApiService.SendDocumentAsync(token, chat_id, document, documentFile);
            return Ok(new { ok = true, result });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in sendDocument");
            return Ok(new { ok = false, error_code = 400, description = ex.Message });
        }
    }

    [HttpPost("bot{botId}:{secretToken}/sendVideo")]
    public async Task<IActionResult> SendVideo(string botId, string secretToken,
        [FromForm] long chat_id, [FromForm] string? video = null, IFormFile? videoFile = null)
    {
        var authError = await ValidateTokenAsync(botId, secretToken);
        if (authError != null) return authError;

        try
        {
            var token = $"{botId}:{secretToken}";
            var result = await botApiService.SendVideoAsync(token, chat_id, video, videoFile);
            return Ok(new { ok = true, result });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in sendVideo");
            return Ok(new { ok = false, error_code = 400, description = ex.Message });
        }
    }

    [HttpPost("bot{botId}:{secretToken}/sendAnimation")]
    public async Task<IActionResult> SendAnimation(string botId, string secretToken,
        [FromForm] long chat_id, [FromForm] string? animation = null, IFormFile? animationFile = null)
    {
        var authError = await ValidateTokenAsync(botId, secretToken);
        if (authError != null) return authError;

        try
        {
            var token = $"{botId}:{secretToken}";
            var result = await botApiService.SendAnimationAsync(token, chat_id, animation, animationFile);
            return Ok(new { ok = true, result });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in sendAnimation");
            return Ok(new { ok = false, error_code = 400, description = ex.Message });
        }
    }

    [HttpPost("bot{botId}:{secretToken}/sendVoice")]
    public async Task<IActionResult> SendVoice(string botId, string secretToken,
        [FromForm] long chat_id, [FromForm] string? voice = null, IFormFile? voiceFile = null)
    {
        var authError = await ValidateTokenAsync(botId, secretToken);
        if (authError != null) return authError;

        try
        {
            var token = $"{botId}:{secretToken}";
            var result = await botApiService.SendVoiceAsync(token, chat_id, voice, voiceFile);
            return Ok(new { ok = true, result });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in sendVoice");
            return Ok(new { ok = false, error_code = 400, description = ex.Message });
        }
    }

    [HttpPost("bot{botId}:{secretToken}/sendVideoNote")]
    public async Task<IActionResult> SendVideoNote(string botId, string secretToken,
        [FromForm] long chat_id, [FromForm] string? video_note = null, IFormFile? videoNoteFile = null)
    {
        var authError = await ValidateTokenAsync(botId, secretToken);
        if (authError != null) return authError;

        try
        {
            var token = $"{botId}:{secretToken}";
            var result = await botApiService.SendVideoNoteAsync(token, chat_id, video_note, videoNoteFile);
            return Ok(new { ok = true, result });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in sendVideoNote");
            return Ok(new { ok = false, error_code = 400, description = ex.Message });
        }
    }

    [HttpPost("bot{botId}:{secretToken}/sendSticker")]
    public async Task<IActionResult> SendSticker(string botId, string secretToken,
        [FromForm] long chat_id, [FromForm] string? sticker = null, IFormFile? stickerFile = null)
    {
        var authError = await ValidateTokenAsync(botId, secretToken);
        if (authError != null) return authError;

        try
        {
            var token = $"{botId}:{secretToken}";
            var result = await botApiService.SendStickerAsync(token, chat_id, sticker, stickerFile);
            return Ok(new { ok = true, result });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in sendSticker");
            return Ok(new { ok = false, error_code = 400, description = ex.Message });
        }
    }

    [HttpPost("bot{botId}:{secretToken}/sendMediaGroup")]
    public Task<IActionResult> SendMediaGroup(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteAsync(botId, secretToken, t => botApiService.SendMediaGroupAsync(t, body), "sendMediaGroup");

    [HttpPost("bot{botId}:{secretToken}/sendLocation")]
    public Task<IActionResult> SendLocation(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteAsync(botId, secretToken, t => botApiService.SendLocationAsync(t, body), "sendLocation");

    [HttpPost("bot{botId}:{secretToken}/sendVenue")]
    public Task<IActionResult> SendVenue(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteAsync(botId, secretToken, t => botApiService.SendVenueAsync(t, body), "sendVenue");

    [HttpPost("bot{botId}:{secretToken}/sendContact")]
    public Task<IActionResult> SendContact(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteAsync(botId, secretToken, t => botApiService.SendContactAsync(t, body), "sendContact");

    [HttpPost("bot{botId}:{secretToken}/sendPoll")]
    public Task<IActionResult> SendPoll(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteAsync(botId, secretToken, t => botApiService.SendPollAsync(t, body), "sendPoll");

    [HttpPost("bot{botId}:{secretToken}/sendDice")]
    public Task<IActionResult> SendDice(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteAsync(botId, secretToken, t => botApiService.SendDiceAsync(t, body), "sendDice");

    #endregion

    #region Chat Action

    [HttpPost("bot{botId}:{secretToken}/sendChatAction")]
    public Task<IActionResult> SendChatAction(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteVoidAsync(botId, secretToken, t => botApiService.SendChatActionAsync(t, body), "sendChatAction");

    #endregion

    #region Edit/Delete

    [HttpPost("bot{botId}:{secretToken}/editMessageText")]
    public Task<IActionResult> EditMessageText(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteAsync(botId, secretToken, t => botApiService.EditMessageTextAsync(t, body), "editMessageText");

    [HttpPost("bot{botId}:{secretToken}/editMessageReplyMarkup")]
    public Task<IActionResult> EditMessageReplyMarkup(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteAsync(botId, secretToken, t => botApiService.EditMessageReplyMarkupAsync(t, body), "editMessageReplyMarkup");

    [HttpPost("bot{botId}:{secretToken}/deleteMessage")]
    public Task<IActionResult> DeleteMessage(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteAsync(botId, secretToken, t => botApiService.DeleteMessageAsync(t, body), "deleteMessage");

    #endregion

    #region Callback Query

    [HttpPost("bot{botId}:{secretToken}/answerCallbackQuery")]
    public Task<IActionResult> AnswerCallbackQuery(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteAsync(botId, secretToken, t => botApiService.AnswerCallbackQueryAsync(t, body), "answerCallbackQuery");

    #endregion

    #region User & File

    [HttpGet("bot{botId}:{secretToken}/getUserProfilePhotos")]
    public async Task<IActionResult> GetUserProfilePhotos(string botId, string secretToken,
        [FromQuery] long user_id, [FromQuery] int? offset = null, [FromQuery] int? limit = null)
    {
        var authError = await ValidateTokenAsync(botId, secretToken);
        if (authError != null) return authError;

        try
        {
            var token = $"{botId}:{secretToken}";
            var result = await botApiService.GetUserProfilePhotosAsync(token, user_id, offset, limit);
            return Ok(new { ok = true, result });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in getUserProfilePhotos");
            return Ok(new { ok = false, error_code = 400, description = ex.Message });
        }
    }

    [HttpGet("bot{botId}:{secretToken}/getFile")]
    public async Task<IActionResult> GetFile(string botId, string secretToken, [FromQuery] string file_id)
    {
        var authError = await ValidateTokenAsync(botId, secretToken);
        if (authError != null) return authError;

        try
        {
            var token = $"{botId}:{secretToken}";
            var result = await botApiService.GetFileAsync(token, file_id);
            return Ok(new { ok = true, result });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in getFile");
            return Ok(new { ok = false, error_code = 400, description = ex.Message });
        }
    }

    [HttpPost("bot{botId}:{secretToken}/getChat")]
    public Task<IActionResult> GetChat(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteAsync(botId, secretToken, t => botApiService.GetChatAsync(t, body), "getChat");

    #endregion

    #region Chat Member Management

    [HttpPost("bot{botId}:{secretToken}/banChatMember")]
    public Task<IActionResult> BanChatMember(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteVoidAsync(botId, secretToken, t => botApiService.BanChatMemberAsync(t, body), "banChatMember");

    [HttpPost("bot{botId}:{secretToken}/unbanChatMember")]
    public Task<IActionResult> UnbanChatMember(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteVoidAsync(botId, secretToken, t => botApiService.UnbanChatMemberAsync(t, body), "unbanChatMember");

    [HttpPost("bot{botId}:{secretToken}/restrictChatMember")]
    public Task<IActionResult> RestrictChatMember(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteVoidAsync(botId, secretToken, t => botApiService.RestrictChatMemberAsync(t, body), "restrictChatMember");

    [HttpPost("bot{botId}:{secretToken}/promoteChatMember")]
    public Task<IActionResult> PromoteChatMember(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteVoidAsync(botId, secretToken, t => botApiService.PromoteChatMemberAsync(t, body), "promoteChatMember");

    [HttpPost("bot{botId}:{secretToken}/setChatAdministratorCustomTitle")]
    public Task<IActionResult> SetChatAdministratorCustomTitle(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteVoidAsync(botId, secretToken, t => botApiService.SetChatAdministratorCustomTitleAsync(t, body), "setChatAdministratorCustomTitle");

    [HttpPost("bot{botId}:{secretToken}/banChatSenderChat")]
    public Task<IActionResult> BanChatSenderChat(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteVoidAsync(botId, secretToken, t => botApiService.BanChatSenderChatAsync(t, body), "banChatSenderChat");

    [HttpPost("bot{botId}:{secretToken}/unbanChatSenderChat")]
    public Task<IActionResult> UnbanChatSenderChat(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteVoidAsync(botId, secretToken, t => botApiService.UnbanChatSenderChatAsync(t, body), "unbanChatSenderChat");

    [HttpPost("bot{botId}:{secretToken}/setChatPermissions")]
    public Task<IActionResult> SetChatPermissions(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteVoidAsync(botId, secretToken, t => botApiService.SetChatPermissionsAsync(t, body), "setChatPermissions");

    #endregion

    #region Invite Links

    [HttpPost("bot{botId}:{secretToken}/exportChatInviteLink")]
    public Task<IActionResult> ExportChatInviteLink(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteAsync(botId, secretToken, t => botApiService.ExportChatInviteLinkAsync(t, body), "exportChatInviteLink");

    [HttpPost("bot{botId}:{secretToken}/createChatInviteLink")]
    public Task<IActionResult> CreateChatInviteLink(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteAsync(botId, secretToken, t => botApiService.CreateChatInviteLinkAsync(t, body), "createChatInviteLink");

    [HttpPost("bot{botId}:{secretToken}/editChatInviteLink")]
    public Task<IActionResult> EditChatInviteLink(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteAsync(botId, secretToken, t => botApiService.EditChatInviteLinkAsync(t, body), "editChatInviteLink");

    [HttpPost("bot{botId}:{secretToken}/revokeChatInviteLink")]
    public Task<IActionResult> RevokeChatInviteLink(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteAsync(botId, secretToken, t => botApiService.RevokeChatInviteLinkAsync(t, body), "revokeChatInviteLink");

    #endregion

    #region Join Requests

    [HttpPost("bot{botId}:{secretToken}/approveChatJoinRequest")]
    public Task<IActionResult> ApproveChatJoinRequest(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteVoidAsync(botId, secretToken, t => botApiService.ApproveChatJoinRequestAsync(t, body), "approveChatJoinRequest");

    [HttpPost("bot{botId}:{secretToken}/declineChatJoinRequest")]
    public Task<IActionResult> DeclineChatJoinRequest(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteVoidAsync(botId, secretToken, t => botApiService.DeclineChatJoinRequestAsync(t, body), "declineChatJoinRequest");

    #endregion

    #region Chat Photo / Title / Description

    [HttpPost("bot{botId}:{secretToken}/setChatPhoto")]
    public async Task<IActionResult> SetChatPhoto(string botId, string secretToken,
        [FromForm] long chat_id, IFormFile photo)
    {
        var authError = await ValidateTokenAsync(botId, secretToken);
        if (authError != null) return authError;

        try
        {
            var token = $"{botId}:{secretToken}";
            await botApiService.SetChatPhotoAsync(token, chat_id, photo);
            return Ok(new { ok = true, result = true });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in setChatPhoto");
            return Ok(new { ok = false, error_code = 400, description = ex.Message });
        }
    }

    [HttpPost("bot{botId}:{secretToken}/deleteChatPhoto")]
    public Task<IActionResult> DeleteChatPhoto(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteVoidAsync(botId, secretToken, t => botApiService.DeleteChatPhotoAsync(t, body), "deleteChatPhoto");

    [HttpPost("bot{botId}:{secretToken}/setChatTitle")]
    public Task<IActionResult> SetChatTitle(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteVoidAsync(botId, secretToken, t => botApiService.SetChatTitleAsync(t, body), "setChatTitle");

    [HttpPost("bot{botId}:{secretToken}/setChatDescription")]
    public Task<IActionResult> SetChatDescription(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteVoidAsync(botId, secretToken, t => botApiService.SetChatDescriptionAsync(t, body), "setChatDescription");

    #endregion

    #region Star Gifts

    [HttpGet("bot{botId}:{secretToken}/getAvailableGifts")]
    public Task<IActionResult> GetAvailableGifts(string botId, string secretToken)
        => ExecuteAsync(botId, secretToken, t => botApiService.GetAvailableGiftsAsync(t), "getAvailableGifts");

    [HttpPost("bot{botId}:{secretToken}/sendGift")]
    public Task<IActionResult> SendGift(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteAsync(botId, secretToken, t => botApiService.SendGiftAsync(t, body), "sendGift");

    #endregion

    #region Stars Payment

    [HttpPost("bot{botId}:{secretToken}/sendInvoice")]
    public Task<IActionResult> SendInvoice(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteAsync(botId, secretToken, t => botApiService.SendInvoiceAsync(t, body), "sendInvoice");

    [HttpPost("bot{botId}:{secretToken}/answerPreCheckoutQuery")]
    public Task<IActionResult> AnswerPreCheckoutQuery(string botId, string secretToken, [FromBody] JsonElement body)
        => ExecuteAsync(botId, secretToken, t => botApiService.AnswerPreCheckoutQueryAsync(t, body), "answerPreCheckoutQuery");

    #endregion
}
