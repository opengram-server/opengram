namespace MyTelegram.Messenger.Services.Impl;

public class UsernameHelper : IUsernameHelper, ITransientDependency
{
    private static readonly string Pattern = "^[a-z0-9_]{5,32}$";
    public bool IsValidUsername(string username)
    {
        return Regex.IsMatch(username, Pattern, RegexOptions.IgnoreCase);
    }
}