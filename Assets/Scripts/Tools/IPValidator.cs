using System.Text.RegularExpressions;
namespace HideAndSeek
{
    public static class IPValidator
    {
        private static readonly Regex ipRegex = new Regex(
            @"^((25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)\.){3}(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)$",
            RegexOptions.Compiled);

        public static bool IsValidIPv4(string ip)
        {
            return ipRegex.IsMatch(ip);
        }
    }
}
