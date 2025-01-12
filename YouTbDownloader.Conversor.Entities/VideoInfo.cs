using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace YouTbDownloader.Conversor.Entities;

public partial class VideoInfo
{
    public Guid GuidId { get; private set; } = Guid.NewGuid();
    public string OriginalTitle { get; }
    public string NormalizedTitle => NormalizeString();
    public VideoInfo(string originalTitle)
    {
        OriginalTitle = originalTitle;
    }

    private string NormalizeString()
    {
        var regex = MyRegex();
        var sanitized = regex.Replace(OriginalTitle, " ");
        var normalized = sanitized.Normalize(NormalizationForm.FormD);
        var result = new StringBuilder();

        foreach (var letter in normalized.Where(letter => CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark))
        {
            result.Append(letter);
        }

        return result.ToString().Normalize(NormalizationForm.FormC);
    }

    [GeneratedRegex(@"[\\\""'\|/]")]
    private static partial Regex MyRegex();
}