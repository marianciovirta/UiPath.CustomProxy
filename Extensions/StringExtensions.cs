namespace UiPath.CustomProxy.Extensions
{
    internal static class StringExtensions
    {
        public static string GetCommandLineParameter(this string input) =>
            input?.Trim().RemoveSinglePrefix("--", "-", "/");

        public static string RemoveSinglePrefix(this string input, params string[] prefixes)
        {
            foreach (var prefix in prefixes)
            {
                var removed = input.RemovePrefix(prefix);
                if (removed != input)
                    return removed;
            }

            return input;
        }

        public static string RemovePrefix(this string input, string prefix) =>
            input?.StartsWith(prefix) == true
                ? input.Substring(prefix.Length)
                : input;
    }
}
