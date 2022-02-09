namespace AniGate.Core.FrameworkExtensions
{
    public static class StringExtensions
    {
        public static Uri? ToUri(this string? uri) =>
            Uri.TryCreate(uri, UriKind.Absolute, out var parsedUri) ? parsedUri : null;
    }
}
