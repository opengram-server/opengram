namespace MyTelegram.Domain.Aggregates.Theme;

[JsonConverter(typeof(SingleValueObjectConverter))]
public class ThemeId : Identity<ThemeId>
{
    public ThemeId(string value) : base(value)
    {
    }
}
