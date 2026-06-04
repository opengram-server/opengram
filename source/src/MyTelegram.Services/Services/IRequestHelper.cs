namespace MyTelegram.Services.Services;

public interface IRequestHelper
{
    /// <summary>
    ///     Check for duplicate requests within 60 seconds
    /// </summary>
    /// <param name="requestInfo"></param>
    /// <returns></returns>
    Task<bool> CheckRequestAsync(IRequestInput requestInfo);
}