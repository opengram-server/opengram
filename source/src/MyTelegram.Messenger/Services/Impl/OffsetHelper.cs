namespace MyTelegram.Messenger.Services.Impl;

public class OffsetHelper : IOffsetHelper, ITransientDependency
{
    public OffsetInfo GetOffsetInfo(GetPagedListInput input)
    {
        return GetOffsetInfo(input.AddOffset, input.OffsetId, input.Limit, input.MinId, input.MaxId, input.MinDate,
            input.MaxDate);
    }

    public OffsetInfo GetOffsetInfo(int addOffset, int offsetId, int limit, int minId, int maxId, int minDate, int maxDate)
    {
        var loadType = GetOffsetLoadType(limit, addOffset);
        var newMaxId = 0;
        var fromId = 0;
        switch (loadType)
        {
            case LoadType.Backward:
                if (offsetId > 0)
                {
                    fromId = offsetId - limit;
                    newMaxId = offsetId + addOffset;
                }

                break;

            case LoadType.Forward:
                fromId = offsetId;
                break;

            case LoadType.FirstUnread:
                break;

            case LoadType.AroundMessage:
                fromId = offsetId + addOffset;
                newMaxId = offsetId + addOffset + limit;
                break;

            case LoadType.AroundDate:
                break;

            case LoadType.LimitIs1:

                break;

            case LoadType.FromUnread:
                break;
            default:
                throw new ArgumentOutOfRangeException($"Unsuporrted load type: {loadType}");
        }

        return new OffsetInfo { OffsetId = offsetId, MaxId = newMaxId, FromId = fromId, LoadType = loadType };
    }

    private static LoadType GetOffsetLoadType(int limit, int addOffset)
    {
        switch (addOffset)
        {
            case 0:
            case -1:
                return LoadType.Backward;

            //case -1:
            case var _ when addOffset == -limit:
            case var _ when addOffset == -limit - 1:
                return LoadType.Forward;

            case var _ when addOffset == -limit / 2:
            case var _ when addOffset == -(limit / 2 + 1):
                return LoadType.AroundMessage;

            case var _ when addOffset == -limit + 5:
                return LoadType.AroundDate;

            case var _ when addOffset == -limit + 10:
            case var _ when addOffset == -limit + 6:
                return LoadType.FromUnread;

            default:
                return LoadType.Forward;
        }

    }
}