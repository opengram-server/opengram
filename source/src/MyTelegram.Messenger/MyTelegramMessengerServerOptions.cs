using System.ComponentModel.DataAnnotations;

namespace MyTelegram.Messenger;
#nullable disable
public class MyTelegramMessengerServerOptions
{
    public string FileServerGrpcServiceUrl { get; set; }
    public string FileServerUploadPath { get; set; } = "uploads";

    [RegularExpression("^([\\d]{3,6})|(\\s*)$")]
    public string FixedVerifyCode { get; set; }

    [Range(3, 6)]
    public int VerificationCodeLength { get; set; } = 5;

    [Range(60, int.MaxValue)]
    public int VerificationCodeExpirationSeconds { get; set; } = 300;
    public string JoinChatDomain { get; set; }

    public int ChannelGetDifferenceIntervalSeconds { get; set; }

    public bool UseInMemoryFilters { get; set; }
    public int EditTimeLimit { get; set; }
    public List<WebRtcConnection> WebRtcConnections { get; set; }
    public int ThisDcId { get; set; }
    public List<DcOption> DcOptions { get; set; }
    public bool AutoCreateSuperGroup { get; set; }
    public bool EnableFutureAuthToken { get; set; }
    public bool SetPremiumToTrueAfterUserCreated { get; set; }
    public bool SendWelcomeMessageAfterUserSignIn { get; set; }
    public bool SetupPasswordRequired { get; set; }
    public bool EnableEmailLogin { get; set; }

    [RegularExpression("^([\\d]{6})|(\\s*)$")]
    public string FixedEmailVerificationCode { get; set; }

    //public long? SupportUserId { get; set; }
    // https://github.com/dotnet/runtime/issues/36510
    [RegularExpression("^([\\d]{1,19})|(\\s*)$")]
    public string SupportUserId { get; set; }
    public int MaxInMemoryContactCount { get; set; }
    public bool CheckPhoneNumberFormat { get; set; }
    public bool EnableSearchNonContacts { get; set; }
    public int RpcResultExpirationMinutes { get; set; }
    
    // Frozen accounts snowflake emoji
    public long? FrozenAccountSnowflakeEmojiId { get; set; }
}