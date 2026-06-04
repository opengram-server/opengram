// ReSharper disable once CheckNamespace
namespace MyTelegram.Domain;

public enum FreezeStatus
{
    Active = 1,          // Активная заморозка
    AppealPending = 2,   // Подана апелляция, ожидает рассмотрения
    Approved = 3,        // Апелляция одобрена (разморожен)
    Rejected = 4,        // Апелляция отклонена
    Expired = 5          // Срок истек, аккаунт удален
}
