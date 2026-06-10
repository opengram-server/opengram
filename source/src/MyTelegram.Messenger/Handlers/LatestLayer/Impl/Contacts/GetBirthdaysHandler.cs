namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Contacts;

///<summary>
/// Fetch all contacts with birthdays that fall within +1/-1 days of the current day.
/// See <a href="https://corefork.telegram.org/method/contacts.getBirthdays" />
///</summary>
internal sealed class GetBirthdaysHandler(
    IQueryProcessor queryProcessor,
    IUserAppService userAppService)
    : RpcResultObjectHandler<MyTelegram.Schema.Contacts.RequestGetBirthdays, MyTelegram.Schema.Contacts.IContactBirthdays>,
    Contacts.IGetBirthdaysHandler
{
    protected override async Task<MyTelegram.Schema.Contacts.IContactBirthdays> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Contacts.RequestGetBirthdays obj)
    {
        // 1. Get all contact user IDs for this user
        var contactUserIds = await queryProcessor.ProcessAsync(
            new GetContactUserIdListQuery(input.UserId),
            CancellationToken.None);

        if (contactUserIds.Count == 0)
        {
            return new TContactBirthdays
            {
                Contacts = [],
                Users = []
            };
        }

        // 2. Load user profiles via cache
        var users = await userAppService.GetListAsync(contactUserIds.ToList());

        // 3. Filter by birthday within ±1 day of today (per corefork spec)
        var today = DateTime.UtcNow;

        var contactBirthdays = new List<Schema.IContactBirthday>();
        var userList = new List<Schema.IUser>();

        foreach (var user in users)
        {
            if (user.Birthday == null)
                continue;

            if (!IsBirthdayWithinRange(user.Birthday, today))
                continue;

            contactBirthdays.Add(new TContactBirthday
            {
                ContactId = user.UserId,
                Birthday = new TBirthday
                {
                    Day = user.Birthday.Day,
                    Month = user.Birthday.Month,
                    Year = user.Birthday.Year
                }
            });

            userList.Add(new TUser
            {
                Id = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.PhoneNumber,
                AccessHash = user.AccessHash
            });
        }

        return new TContactBirthdays
        {
            Contacts = new TVector<Schema.IContactBirthday>(contactBirthdays),
            Users = new TVector<Schema.IUser>(userList)
        };
    }

    private static bool IsBirthdayWithinRange(Birthday birthday, DateTime today)
    {
        try
        {
            var todayDayOfYear = today.DayOfYear;
            var daysInYear = DateTime.IsLeapYear(today.Year) ? 366 : 365;

            var birthdayDate = new DateTime(today.Year, birthday.Month, birthday.Day);
            var birthdayDayOfYear = birthdayDate.DayOfYear;

            var diff = Math.Abs(birthdayDayOfYear - todayDayOfYear);
            // Handle year wrap (e.g., Dec 31 <-> Jan 1)
            if (diff > daysInYear / 2)
                diff = daysInYear - diff;

            return diff <= 1;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
    }
}
