using System;

namespace AniCore.WpfClient.Common;

public sealed class WebPlayerViewModel
{
    public WebPlayerViewModel(Uri url)
    {
        Url = url;
    }

    public Uri Url { get; }
}