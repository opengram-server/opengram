namespace MyTelegram.QueryHandlers.InMemory;

public class GetAllChatThemesQueryHandler : IQueryHandler<GetAllChatThemesQuery, IReadOnlyList<Theme>>
{
    public Task<IReadOnlyList<Theme>> ExecuteQueryAsync(GetAllChatThemesQuery query, CancellationToken cancellationToken)
    {
        // Преднастроенные темы чата со своими эмодзи
        var themes = new List<Theme>
        {
            CreateTheme(1, "🏠", "Home", "tdesktop"),
            CreateTheme(2, "❤️", "Love", "tdesktop"),
            CreateTheme(3, "🎨", "Art", "tdesktop"),
            CreateTheme(4, "🌙", "Night", "tdesktop"),
            CreateTheme(5, "🌊", "Ocean", "tdesktop"),
            CreateTheme(6, "🌸", "Blossom", "tdesktop"),
            CreateTheme(7, "⚡", "Energy", "tdesktop"),
            CreateTheme(8, "🔥", "Fire", "tdesktop"),
            CreateTheme(9, "🌲", "Forest", "tdesktop"),
            CreateTheme(10, "☁️", "Cloud", "tdesktop"),
            CreateTheme(11, "🍭", "Candy", "tdesktop"),
            CreateTheme(12, "🌈", "Rainbow", "tdesktop")
        };

        return Task.FromResult<IReadOnlyList<Theme>>(themes);
    }

    private static Theme CreateTheme(long id, string emoticon, string title, string format)
    {
        return new Theme(
            Default: false,
            ForChat: true,
            Id: id,
            AccessHash: id * 1000,
            Slug: $"chat-theme-{id}",
            Title: title,
            DocumentId: null,
            Settings: new List<ThemeSettings>(),
            Emoticon: emoticon,
            Format: format
        );
    }
}
