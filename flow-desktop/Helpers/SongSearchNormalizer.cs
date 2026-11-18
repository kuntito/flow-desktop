using System.Text.RegularExpressions;

namespace flow_desktop.Helpers
{
    public static class SongSearchNormalizer
    {
        private static readonly Regex NormalizeRegex = new(
            "[^a-zA-Z0-9]",
            RegexOptions.Compiled
        );

        public static string Normalize(string text)
        {
            var lettersAndDigits = NormalizeRegex.Replace(text, "");

            return lettersAndDigits.ToLowerInvariant();
        }
    }
}