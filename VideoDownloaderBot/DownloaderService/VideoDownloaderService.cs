using System.Text.RegularExpressions;

namespace Xoggas.VideoDownloaderBot;

public sealed class VideoDownloaderService
{
    private readonly Dictionary<Regex, IVideoDownloader> _videoDownloaders = new();

    public void AddDownloader(string regexPattern, IVideoDownloader downloader)
    {
        _videoDownloaders.Add(new Regex(regexPattern), downloader);
    }

    public IVideoDownloader? GetDownloader(string uri)
    {
        return _videoDownloaders.FirstOrDefault(x => x.Key.IsMatch(uri)).Value;
    }
}