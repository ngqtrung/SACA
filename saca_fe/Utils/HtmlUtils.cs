using HtmlAgilityPack;
using System.Net;

public static class HtmlUtils
{
    public static string TruncateHtmlToPlainText(string html, int maxLength)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        // 1. Remove <img> tags
        var imgNodes = doc.DocumentNode.SelectNodes("//img");
        if (imgNodes != null)
        {
            foreach (var img in imgNodes)
            {
                img.Remove();
            }
        }

        // 2. Extract plain text
        string plainText = doc.DocumentNode.InnerText;

        if (plainText.Length <= maxLength)
            return doc.DocumentNode.OuterHtml;

        string truncated = System.Net.WebUtility.HtmlEncode(plainText.Substring(0, maxLength) + "...");
        return $"<span>{truncated}</span>";
    }

    public static string DecodeHtmlEntities(string input)
    {
        return WebUtility.HtmlDecode(input);
    }
}
