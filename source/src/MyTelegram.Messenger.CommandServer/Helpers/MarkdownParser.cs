using MyTelegram.Schema;
using System.Text.RegularExpressions;

namespace MyTelegram.Messenger.CommandServer.Helpers;

/// <summary>
/// Simple Markdown parser for Bot API messages
/// </summary>
public static class MarkdownParser
{
    public static List<IMessageEntity>? ParseMarkdown(string text)
    {
        if (string.IsNullOrEmpty(text))
            return null;

        var entities = new List<IMessageEntity>();

        // Parse **bold** or *bold*
        var boldPattern = @"\*\*(.+?)\*\*|\*(.+?)\*";
        foreach (Match match in Regex.Matches(text, boldPattern))
        {
            var content = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;
            var offset = match.Index;
            
            entities.Add(new TMessageEntityBold
            {
                Offset = offset,
                Length = match.Length
            });
        }

        // Parse `code`
        var codePattern = @"`(.+?)`";
        foreach (Match match in Regex.Matches(text, codePattern))
        {
            entities.Add(new TMessageEntityCode
            {
                Offset = match.Index,
                Length = match.Length
            });
        }

        return entities.Count > 0 ? entities : null;
    }
}
