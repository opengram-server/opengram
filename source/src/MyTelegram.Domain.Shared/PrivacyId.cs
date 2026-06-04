namespace MyTelegram;

public static class PrivacyId
{
    public static string Create(long userId, PrivacyType privacyType)
    {
        return $"{userId}:{(int)privacyType}";
    }
}
