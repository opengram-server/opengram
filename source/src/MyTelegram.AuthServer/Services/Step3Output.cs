namespace MyTelegram.AuthServer.Services;

public record Step3Output(
    long AuthKeyId,
    byte[] AuthKey,
    long ServerSalt,
    bool IsPermanent,
    ISetClientDHParamsAnswer SetClientDhParamsAnswer,
    int? DcId = null
);