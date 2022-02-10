using System.Diagnostics;

namespace AniGate.Core
{
    public static class EpisodeExtensions
    {
        public static void OpenInBrowser(this Episode episode) =>
            Process.Start(new ProcessStartInfo(episode.VoeLink)
            {
                UseShellExecute = true
            });
    }
}
