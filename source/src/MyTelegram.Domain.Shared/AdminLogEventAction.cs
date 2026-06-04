namespace MyTelegram;

public record AdminLogEventActionData(string NewValue, string? PrevValue = null);

public enum AdminLogEventAction
{
    All,
    ChangeAbout,
    ChangeAvailableReactions,
    ChangeHistoryTTL,
    ChangeLinkedChat,
    ChangeLocation,
    ChangePhoto,
    ChangeStickerSet,
    ChangeTitle,
    ChangeUsername,
    DefaultBannedRights,
    DeleteMessage,
    DiscardGroupCall,
    EditMessage,
    ExportedInviteDelete,
    ExportedInviteEdit,
    ExportedInviteRevoke,
    ParticipantInvite,
    ParticipantJoin,
    ParticipantJoinByInvite,
    ParticipantJoinByRequest,
    ParticipantLeave,
    ParticipantMute,
    ParticipantToggleAdmin,
    ParticipantToggleBan,
    ParticipantUnmute,
    ParticipantVolume,
    SendMessage,
    StartGroupCall,
    StopPoll,
    ToggleGroupCallSetting,
    ToggleInvites,
    ToggleNoForwards,
    TogglePreHistoryHidden,
    ToggleSignatures,
    ToggleSlowMode,
    UpdatePinned
}