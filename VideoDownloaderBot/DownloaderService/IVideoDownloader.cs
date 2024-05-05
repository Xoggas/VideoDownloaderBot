namespace Xoggas.VideoDownloaderBot;

public interface IVideoDownloader
{
    public string ServiceName { get; }
    public Task<Stream> Download(string videoUri);
}