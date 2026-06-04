namespace MyTelegram;

public class PasswordSetting(
    PasswordKdfAlgoSha256Sha256Pbkdf2Hmacsha512Iter100000Sha256ModPow newAlgo,
    byte[] newPasswordHash,
    string hint,
    string email,
    SecureSecretSetting newSecureSettings)

{
    public string Email { get; init; } = email;
    public string Hint { get; init; } = hint;

    public PasswordKdfAlgoSha256Sha256Pbkdf2Hmacsha512Iter100000Sha256ModPow NewAlgo { get; init; } = newAlgo;

    public byte[] NewPasswordHash { get; init; } = newPasswordHash;
    public SecureSecretSetting NewSecureSettings { get; init; } = newSecureSettings;
}