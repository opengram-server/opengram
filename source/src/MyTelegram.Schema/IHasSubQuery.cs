namespace MyTelegram.Schema;

public interface IHasSubQuery
{
    IObject Query { get; set; }

    public uint GetObjectId()
    {
        if (Query is IHasSubQuery subQuery)
        {
            return subQuery.GetObjectId();
        }

        return Query.ConstructorId;
    }
}